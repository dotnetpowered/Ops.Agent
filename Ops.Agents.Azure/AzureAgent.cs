using System.Reflection;
using Azure;
using Azure.Identity;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Ops.Agents.Azure;

public class AzureAgent : IOpsAgent
{
    private readonly ILogger<AzureAgent> _logger;
    private readonly IOpsIngestApi _ingestApi;

    public AzureAgent(ILogger<AzureAgent> logger, IOpsIngestApi ingestApi)
    {
        _logger = logger;
        _ingestApi = ingestApi;
    }

    public string SourceName => "azure";

    public async Task CollectAsync(AgentConfig agentConfig)
    {
        //https://docs.microsoft.com/en-us/azure/automation/update-management/query-logs
        var assembly = Assembly.GetExecutingAssembly();
        var stream = assembly.GetManifestResourceStream("Computer_List.txt");
        if (stream == null)
            throw new InvalidOperationException("Unable to load Computer_List.txt resource.");
        var streamreader = new StreamReader(stream);
        var query = streamreader.ReadToEnd();
        var client = new LogsQueryClient(new DefaultAzureCredential(true));
        var response = await client.QueryWorkspaceAsync(
            agentConfig.WorkspaceId, query,
            new QueryTimeRange(TimeSpan.FromDays(1)));

        LogsTable table = response.Value.Table;

        List<UpdateAssessment> items = new();
        foreach (var row in table.Rows)
        {
            var item = new UpdateAssessment((string)row["id"], (string)row["displayName"])
            {
                LastAccessedTime = (DateTimeOffset)row["lastAssessedTime"],
                Compliance = (long)row["compliance"] == 1,
                ComputerEnvironment = (string)row["environment"],
                MissingCriticalUpdatesCount = (long)row["missingCriticalUpdatesCount"],
                MissingOtherUpdatesCount = (long)row["missingOtherUpdatesCount"],
                MissingSecurityUpdatesCount = (long)row["missingSecurityUpdatesCount"],
            };
            items.Add(item);
        }
        //await dbClient.UpsertItemsAsync("Metrics", "Items", items);
    }

}