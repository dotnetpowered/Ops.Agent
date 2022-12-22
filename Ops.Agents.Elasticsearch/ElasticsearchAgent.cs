using System.Text.Json;
using Microsoft.Extensions.Logging;
using Ops.Agents.Elasticsearch.Models;
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
        using var client = new HttpClient();

        Uri esUri = new Uri(agentConfig.Url);
        _logger.LogInformation($"Checking cluster health at {esUri}");
        var esContent = await client.GetStringAsync(esUri);
        var esJson = JsonDocument.Parse(esContent);
        var es = JsonSerializer.Deserialize<EsInfo>(esJson);

        Uri healthUri = new Uri($"{agentConfig.Url}/_cluster/health");
        _logger.LogInformation($"Checking cluster health at {healthUri}");
        var healthContent = await client.GetStringAsync(healthUri);
        var healthJson = JsonDocument.Parse(healthContent);
        var clusterHealth = JsonSerializer.Deserialize<EsClusterHealth>(healthJson);

        Uri indicesUri = new Uri($"{agentConfig.Url}/_cat/indices?format=json");
        _logger.LogInformation($"Gathering indices from {indicesUri}");
        var indicesContent = await client.GetStringAsync(indicesUri);
        var indicesJson = JsonDocument.Parse(indicesContent);
        var indices = JsonSerializer.Deserialize<EsIndex[]>(indicesJson);

        Uri nodesUri = new Uri($"{agentConfig.Url}/_nodes");
        _logger.LogInformation($"Gathering nodes from {nodesUri}");
        var nodesContent = await client.GetStringAsync(nodesUri);
        var nodesJson = JsonDocument.Parse(nodesContent);

        var services = new List<DataService>();
        var nodes = nodesJson.RootElement.GetProperty("nodes");
        foreach (var nodeElement in nodes.EnumerateObject())
        {
            var node = JsonSerializer.Deserialize<EsNode>(nodeElement.Value);

            var id = $"sql-{es.cluster_name}-{node.name}";
            var dataService = new DataService(id, this.SourceName, node.name, "Search", "Elasticsearch")
            {
                Architecture = node.os.arch,
                NumCpu = node.os.available_processors,
                OnlineRespositories = indices.Length,
                Status = clusterHealth.status,
                Version = es.version.number,
                ClusterName = es.cluster_name,
                HighAvailability = "Active Cluster"
            };
            services.Add(dataService);
        }
        await _ingestApi.IngestResource(services);
    }
}
