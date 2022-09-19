using System;
using Google.Cloud.Compute.V1;
using Microsoft.Extensions.Logging;

namespace Ops.Agents.Google.Cloud;

public class GoogleComputeAgent : IOpsAgent
{
    private readonly ILogger<GoogleComputeAgent> _logger;
    private readonly IOpsIngestApi _ingestApi;

    public GoogleComputeAgent(ILogger<GoogleComputeAgent> logger, IOpsIngestApi ingestApi)
    {
        _logger = logger;
        _ingestApi = ingestApi;
    }

    public string SourceName => "Google.Compute";

    public Task CollectAsync(AgentConfig agentConfig)
    {
        throw new NotImplementedException();
    }
}

