using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using Ops.Agents.Shared.Models;

namespace Ops.Agents.MongoDB;

public class MongoDbAgent : IOpsAgent
{
    private readonly ILogger<MongoDbAgent> _logger;
    private readonly IOpsIngestApi _ingestApi;

    public MongoDbAgent(ILogger<MongoDbAgent> logger, IOpsIngestApi ingestApi)
    {
        _logger = logger;
        _ingestApi = ingestApi;
    }

    public string SourceName => "MongoDB";

    public async Task CollectAsync(AgentConfig agentConfig)
    {
        try
        {
            //https://www.mongodb.com/docs/manual/reference/command/serverStatus/
            //https://www.mongodb.com/docs/drivers/csharp/
            //https://zetcode.com/csharp/mongodb/
            //https://cloud.mongodb.com/
            var settings = MongoClientSettings.FromConnectionString(agentConfig.ConnectionString);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            var client = new MongoClient(settings);
            var db = client.GetDatabase("test");
            var command = new BsonDocument { { "serverStatus", 1 } };
            var result = db.RunCommand<BsonDocument>(command);
            var id = result["host"].ToString();
            var host = id.Split(':')[0];
            var version = result["version"].ToString();
            var server = new DataService(id, this.SourceName, host, "DocumentDB", "Elasticsearch")
            {
            };
            Console.WriteLine(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        throw new NotImplementedException();
    }

    void Connect()
    {

    }
}
