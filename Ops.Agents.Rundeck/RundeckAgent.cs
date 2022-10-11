using Microsoft.Extensions.Logging;
using Rundeck.Api;
using Rundeck.Api.Models;
using Rundeck.Api.Models.Dtos;
using Ops.Agents.Shared.Models;

namespace Ops.Agents.Rundeck;

public class RundeckAgent : IOpsAgent
{
    private readonly ILogger<RundeckAgent> _logger;
    private readonly IOpsIngestApi _ingestApi;

    public RundeckAgent(ILogger<RundeckAgent> logger, IOpsIngestApi ingestApi)
    {
        _logger = logger;
        _ingestApi = ingestApi;
    }

    public string SourceName => "PagerDuty.Rundeck";

    public async Task CollectAsync(AgentConfig agentConfig)
    {
        RundeckClientOptions options = new()
        {
            ApiToken = agentConfig.ApiToken,
            Uri = new Uri(agentConfig.Url)
        };
        var rundeckClient = new RundeckClient(options);

        var resourceList = new List<object>();
        var systemInfo = await rundeckClient.System.GetSystemInfoAsync();
        var system = systemInfo.System;
        
        var service = new Service(system.Rundeck.ServerUUID, this.SourceName, system.Rundeck.Node, "Job Automation", "PagerDuty RunDeck")
        {
            Version = system.Rundeck.Version,
            Architecture = system.OsInfo.Arch,
            OSName = system.OsInfo.Name + " " + system.OsInfo.Version,
            Status = system.Executions.Active ? "Online" : "Offline",
            ServiceStartTime = system.Stats.Uptime.Since.Datetime.DateTime,
            NumCpu = system.Stats.Cpu.Processors,
            MemoryUsageMB = system.Stats.Memory.Total / 1024 / 1024
        };
        resourceList.Add(service);

        var projects = await rundeckClient.Projects.GetAllAsync();
        var machines = await CollectMachinesAsync(rundeckClient, projects);
        //await CollectJobsAsync(rundeckClient, projects);
        resourceList.AddRange(machines);

        await _ingestApi.IngestResource(resourceList);

    }

    private async Task CollectJobsAsync(RundeckClient rundeckClient, List<ProjectListingDto> projects)
    {
        var jobs = new List<Shared.Models.Job>();
        foreach (var project in projects)
        {
            var rundeckJobs = await rundeckClient.Jobs.GetAllAsync(project.Name);

            foreach (var j in rundeckJobs)
            {
                //var job = new Shared.Models.Job(job.Id, this.SourceName, )
                _logger.LogInformation($"Collecting Executions for job: {project.Name} {j.Name}");
                //var executions = await rundeckClient.Jobs.GetExecutionsAsync(job.Id, 0, 1);

                //foreach (var e in executions.Executions)
                //{
                //    jobExecutions.AddRange(ToJobExecution(e, e.SuccessfulNodes, "Success"));
                //    jobExecutions.AddRange(ToJobExecution(e, e.FailedNodes, "Failure"));
                //}
            }
        }
      //  await _ingestApi.IngestResource(jobExecutions);
    }

    //private IEnumerable<JobExecution> ToJobExecution(Execution e, IList<string> nodes, string executionStatus)
    //{
    //    if (nodes == null)
    //        return Array.Empty<JobExecution>();

    //    return from node in nodes
    //           select new JobExecution(e.Job.Id.ToString() + "-" + node, this.SourceName, node, e.Job.Name)
    //           {
    //               ExecutionId = e.Id.ToString(),
    //               Status = executionStatus,
    //               StartTime = e.DateStarted.Date,
    //               StopTime = e.DateEnded.Date
    //           };
    //}

    private async Task<List<Machine>> CollectMachinesAsync(RundeckClient rundeckClient, List<ProjectListingDto> projects)
    {
        var machines = new List<Machine>();
        foreach (var project in projects)
        {
            var c = project.Config;
            // Get list of machines associated with project
            var resources = await rundeckClient.Projects.GetResourcesAsync(project.Name);
            var projectMachines = from r in resources
                                  select new Machine(r.Key, this.SourceName, r.Value.Hostname)
                                  {
                                      OSName = r.Value.OsName,
                                      Architecture = r.Value.OsArch,
                                      Tags = r.Value.Tags,
                                      Description = r.Value.Description
                                  };
            // Don't add duplicates to list.
            machines.AddRange(from pm in projectMachines
                              where !machines.Exists(m => m.Id == pm.Id)
                              select pm);
        }
        return machines;
    }
}

