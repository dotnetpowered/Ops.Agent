using System.Net.Http;
using Microsoft.Extensions.Logging;
using Refit;

namespace Ops.Agents.RedHat.Insights;

public class InsightsAgent : IOpsAgent
{
    readonly ILogger _logger;

    public string SourceName => "redhat.insights";

    public InsightsAgent(ILogger logger)
    {
        _logger = logger;
    }

    public Task CollectAsync(AgentConfig agentConfig)
    {
        var insightsClient = new InsightsClient(_logger,
            agentConfig.Username, agentConfig.Password);

        throw new NotImplementedException();
    }
}

