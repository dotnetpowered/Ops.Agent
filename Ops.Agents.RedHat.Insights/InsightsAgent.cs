using System.Net.Http;
using Microsoft.Extensions.Logging;
using Refit;
using Ops.Agents.Shared.Models;

namespace Ops.Agents.RedHat.Insights;

public class InsightsAgent : IOpsAgent
{
    readonly ILogger<InsightsClient> _logger;

    public string SourceName => "redhat.insights";

    public InsightsAgent(ILogger<InsightsClient> logger)
    {
        _logger = logger;
    }

    public async Task CollectAsync(AgentConfig agentConfig)
    {
        var insightsClient = new InsightsClient(_logger,
            agentConfig.Username, agentConfig.Password);

        var machines = new List<Machine>();
        var hosts = await insightsClient.Hosts.GetAsync();
        foreach (var host in hosts.Results)
        {
            var h = await insightsClient.Hosts.GetHostAsync(host.Id);
            var rhelHost = h.Results.FirstOrDefault().SystemProfile;
            var m = new Machine(host.Id, this.SourceName, host.Fqdn)
            {
                MemoryGB = (int)(rhelHost.SystemMemoryBytes / 1024 / 1024),
                Architecture = rhelHost.Architecture,
                NumCpu = rhelHost.NumberOfSockets,
                OSName = $"RHEL {rhelHost.OsRelease}",
                Platform = rhelHost.InfrastructureVendor,
                Description = host.DisplayName
            };
            machines.Add(m);
        }
    }
}

