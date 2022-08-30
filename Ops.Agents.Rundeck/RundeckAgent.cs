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

        var projects = await rundeckClient.Projects.GetAllAsync();
        await CollectMachinesAsync(rundeckClient, projects);
        await CollectExecutionsAsync(rundeckClient, projects);
    }

    private static async Task CollectExecutionsAsync(RundeckClient rundeckClient, List<ProjectListingDto> projects)
    {
        var jobExecutions = new List<JobExecution>();
        foreach (var project in projects)
        {
            var jobs = await rundeckClient.Jobs.GetAllAsync(project.Name);

            foreach (var job in jobs)
            {
                var executions = await rundeckClient.Jobs.GetExecutionsAsync(job.Id, 0, 5);

                foreach (var e in executions.Executions)
                {
                    jobExecutions.AddRange(ToJobExecution(e, e.SuccessfulNodes, "Success"));
                    jobExecutions.AddRange(ToJobExecution(e, e.FailedNodes, "Failed"));
                }
            }
        }
        //await dbClient.UpsertItemsAsync("Metrics", "Items", jobExecutions);
    }

    private static IEnumerable<JobExecution> ToJobExecution(Execution e, IList<string> nodes, string executionStatus)
    {
        if (nodes == null)
            return Array.Empty<JobExecution>();

        return from node in nodes
               select new JobExecution(e.Id.ToString() + "-" + node, node, e.Job.Name)
               {
                   Status = executionStatus,
                   StartTime = e.DateStarted.Date,
                   StopTime = e.DateEnded.Date
               };
    }

    private static async Task CollectMachinesAsync(RundeckClient rundeckClient, List<ProjectListingDto> projects)
    {
        var machines = new List<Machine>();
        foreach (var project in projects)
        {
            var c = project.Config;
            // Get list of machines associated with project
            var resources = await rundeckClient.Projects.GetResourcesAsync(project.Name);
            var projectMachines = from r in resources
                                  select new Machine(r.Key, r.Value.Hostname)
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
        //await dbClient.UpsertItemsAsync("Metrics", "Items", machines);
    }
}

