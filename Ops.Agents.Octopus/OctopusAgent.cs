using Microsoft.Extensions.Logging;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusMachineResource = Octopus.Client.Model.MachineResource;
using Ops.Agents.Shared.Models;

namespace Ops.Agents.Octopus;

public class OctopusAgent : IOpsAgent
{
    private readonly ILogger<OctopusAgent> _logger;
    private readonly IOpsIngestApi _ingestApi;

    public OctopusAgent(ILogger<OctopusAgent> logger, IOpsIngestApi ingestApi)
    {
        _logger = logger;
        _ingestApi = ingestApi;
    }

    public string SourceName => "Octopus.Deploy";

    public async Task CollectAsync(AgentConfig agentConfig)
    {
        // Create repository object
        var endpoint = new OctopusServerEndpoint(agentConfig.Url, agentConfig.ApiToken);
        var repository = new OctopusRepository(endpoint);
        var client = new OctopusClient(endpoint);

        var (dashboard, allMachines, deploymentResources) = GetOctopusResources(repository);
        await CollectMachines(allMachines);
        await CollectDeployments(dashboard, allMachines, deploymentResources);
    }

    private async Task CollectDeployments(DashboardResource dashboard,
        List<OctopusMachineResource> allMachines,
        List<DeploymentResource> deploymentResources
        )
    {
        var deployments = new List<DeployedService>();
        foreach (var item in dashboard.Items)
        {
            var deploymentResource = deploymentResources.Find(d => d.Id == item.DeploymentId);
            var machines = allMachines.FindAll(m => deploymentResource.DeployedToMachineIds.Contains(m.Id));
            foreach (var machine in machines)
            {
                var project = dashboard.Projects.Find(p => p.Id == item.ProjectId);
                var environment = dashboard.Environments.Find(e => e.Id == item.EnvironmentId);
                var machineName = new Uri(machine.Uri).Host;
                var deployment = new DeployedService(item.ProjectId+'-'+machineName, this.SourceName, machineName, project.Name)
                {
                    DeploymentId = item.DeploymentId,
                    Version = item.ReleaseVersion,
                    StartTime = item.StartTime,
                    StopTime = item.CompletedTime,
                    Status = item.State.ToString(),
                    Environment = environment.Name,
                    DeployedBy = deploymentResource.DeployedBy,
                };
                deployments.Add(deployment);
            }
            if (!machines.Any())
                _logger.LogWarning($"No machines found for {item.Id}. Skipping.");
        }

        await _ingestApi.IngestResource(deployments);
    }

    private static (DashboardResource dashboard, List<OctopusMachineResource> allMachines,
        List<DeploymentResource> deploymentResources) GetOctopusResources(OctopusRepository repository)
    {
        var allMachines = repository.Machines.FindAll();
        var dashboard = repository.Dashboards.GetDashboard();
        var ids = (from d in dashboard.Items select d.Id).ToArray();
        var chunks = ids.Chunk(30);
        var deploymentResources = new List<DeploymentResource>();
        foreach (var chunk in chunks)
        {
            deploymentResources.AddRange(repository.Deployments.Get(chunk));
        }

        return (dashboard, allMachines, deploymentResources);
    }

    public async Task CollectMachines(List<OctopusMachineResource> allMachines)
    {
        var uri = new Uri(allMachines.Last().Uri);
        var deploymentTargets = from m in allMachines
                                select new DeploymentTarget(m.Id, this.SourceName, new Uri(m.Uri).Host)
                                {
                                    Group = (from e in m.EnvironmentIds select e).ToList(),
                                    OSName = m.OperatingSystem,
                                    Description = String.Join(',', from r in m.Roles select r),
                                    PowerState = m.HealthStatus.ToString(),
                                    Architecture = m.Architecture
                                };

        //await _ingestApi.UpsertResource(deploymentTargets);
    }

}

