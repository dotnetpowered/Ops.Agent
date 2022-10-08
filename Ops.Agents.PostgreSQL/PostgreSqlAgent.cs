using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Ops.Agents.PostgreSQL;

public class PostgreSqlAgent : IOpsAgent
{
    private readonly ILogger<PostgreSqlAgent> _logger;
    private readonly IOpsIngestApi _ingestApi;

    public PostgreSqlAgent(ILogger<PostgreSqlAgent> logger, IOpsIngestApi ingestApi)
    {
        _logger = logger;
        _ingestApi = ingestApi;
    }

    public string SourceName => "PostgreSQL";

    public async Task CollectAsync(AgentConfig agentConfig)
    {
        try
        {
            //https://www.postgresql.org/docs/9.5/functions-info.html

            //version() - db + os + architecture info
            //inet_server_port()
            //inet_server_addr()


            //SELECT datname FROM pg_database;

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
