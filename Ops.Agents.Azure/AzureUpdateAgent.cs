using System.Reflection;
using Azure.Identity;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;
using Microsoft.Extensions.Logging;
using Ops.Agents.Shared.Models;

namespace Ops.Agents.Azure;

// -----------------------------------------------------------------------------
// Azure Query Documentation
// -----------------------------------------------------------------------------
// https://docs.microsoft.com/en-us/azure/automation/update-management/query-logs

public class AzureUpdateAgent : IOpsAgent
{
    private readonly ILogger<AzureUpdateAgent> _logger;
    private readonly IOpsIngestApi _ingestApi;

    public AzureUpdateAgent(ILogger<AzureUpdateAgent> logger, IOpsIngestApi ingestApi)
    {
        _logger = logger;
        _ingestApi = ingestApi;
    }

    public string SourceName => "Azure.Update";

    public async Task CollectAsync(AgentConfig agentConfig)
    {
        // To get a list of resource names:
        // var names = this.GetType().GetTypeInfo().Assembly.GetManifestResourceNames();
        var stream = this.GetType().GetTypeInfo().Assembly.GetManifestResourceStream("Ops.Agents.Azure.ComputerUpdateSummary.txt");
        if (stream == null)
            throw new InvalidOperationException("Unable to load ComputerUpdateSummary.txt resource.");
        var streamreader = new StreamReader(stream);
        var query = streamreader.ReadToEnd();
        var creds = new ClientSecretCredential(agentConfig.Tenant, agentConfig.Username, agentConfig.Password);
        var client = new LogsQueryClient(creds); // new DefaultAzureCredential(true));
        var response = await client.QueryWorkspaceAsync(agentConfig.Resource, query,
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