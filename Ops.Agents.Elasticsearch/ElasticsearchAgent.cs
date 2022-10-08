using System;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Ops.Agents.Shared.Models;

namespace Ops.Agents.Elasticsearch;

public class ElasticsearchAgent : IOpsAgent
{
    private readonly ILogger<ElasticsearchAgent> _logger;
    private readonly IOpsIngestApi _ingestApi;

    public ElasticsearchAgent(ILogger<ElasticsearchAgent> logger, IOpsIngestApi ingestApi)
    {
        _logger = logger;
        _ingestApi = ingestApi;
    }

    public string SourceName => "Elasticsearch";


    public async Task CollectAsync(AgentConfig agentConfig)
    {
        // _cluster/health
        // _cluster/state
        // _nodes


        throw new NotImplementedException();
    }
}
