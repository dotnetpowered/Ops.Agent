using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Ops.Agents.Shared;
using Ops.Agents.Shared.Models;

namespace Ops.Agents.SqlServer;

public class SqlServerAgent : IOpsAgent
{
    private readonly ILogger<SqlServerAgent> _logger;
    private readonly IOpsIngestApi _ingestApi;

    public SqlServerAgent(ILogger<SqlServerAgent> logger, IOpsIngestApi ingestApi)
    {
        _logger = logger;
        _ingestApi = ingestApi;
    }

    public string SourceName => "SqlServer";

    public async Task CollectAsync(AgentConfig agentConfig)
    {
        List<DataServer> servers = new();
        List<SqlNode> nodes = new();

        // Open connection to SQL Server
        using var sqlConnection = new SqlConnection(agentConfig.ConnectionString);
        await sqlConnection.OpenAsync();

        // Collect list of cluster nodes (if applicable)
        using var sqlCmd = sqlConnection.CreateCommand();
        string query = ResourceUtils.LoadEmbeddedResource<SqlServerAgent>("SqlClusterNodes.sql");
        sqlCmd.CommandText = query;
        using (var sqlReader = await sqlCmd.ExecuteReaderAsync())
        {
            var reader = new TypedSqlReader(sqlReader);
            while (reader.Read())
            {
                var node = new SqlNode() {
                    NodeName = reader.GetString("NodeName"),
                    CurrentNode = reader.GetValue<bool>("CurrentNode"),
                    Status = reader.GetString("Status")
                };
                nodes.Add(node);
            }
            await sqlReader.CloseAsync();
        }

        // Create list of machines (either 1 or multiple if cluster)
        query = ResourceUtils.LoadEmbeddedResource<SqlServerAgent>("SqlSystemInfo.sql");
        sqlCmd.CommandText = query;
        using (var sqlReader = await sqlCmd.ExecuteReaderAsync())
        {
            var reader = new TypedSqlReader(sqlReader);
            reader.Read();
            if (reader.GetValue<int>("IsClustered") == 0)
            {
                nodes.Add(new SqlNode() {
                    NodeName = reader.GetString("MachineName").ToLower(),
                    InstanceName = reader.GetString("InstanceName"),
                    CurrentNode = true, Status = "Online" });
            }
            foreach (var node in nodes)
            {
                var server = ToDataServer(reader, node);
                servers.Add(server);
            }
        }

        // Send results to API endpoint
        await _ingestApi.IngestResource(servers);

        // Not used at this time as the data is all in the "version" column:
        // ProductVersion
        // Edition
        // ProductLevel
        // ProductUpdateReference
        // EngineEdition

        // TO DO: implement in the future:

        // AgentStatus (SQL Server Agent)
        // Blocks 
        // You want to see 0 for process_physical_memory_low & process_virtual_memory_low
        //     This indicates that you are not under internal memory pressure

    }

    private DataServer ToDataServer(TypedSqlReader reader, SqlNode node)
    {
        string machineName = node.NodeName.ToLower();
        string instanceName = node.InstanceName.ToLower();
        bool isClustered = reader.GetValue<int>("IsClustered") == 1;
        string clusterName = isClustered ? reader.GetString("ServerName").ToLower() : null;

        string id;
        if (isClustered)
            id = $"sql-{clusterName}-{machineName}";
        else if (string.IsNullOrEmpty(instanceName))
            id = $"sql-{machineName}-0";
        else
            id = $"sql-{machineName}-{instanceName}";

        DataServer server = new (id, this.SourceName, machineName)
        {
            ClusterName = clusterName,
            ActiveNode = node.CurrentNode,
            Status = node.Status,
            InstanceName = instanceName,
            Language = reader.GetString("Language"),
            ServerStartTime = reader.GetValue<DateTime>("StartTime"),
            NumCpu = reader.GetValue<int>("CpuCount"),
            MemoryGB = (int)(reader.GetValue<long>("PhysicalMemoryMB") / 1024),
            MemoryUsageMB = reader.GetValue<long>("UsedMemoryMB"),
            OnlineRespositories = reader.GetValue<int>("OnlineDatabases"),
            OfflineRespositories = reader.GetValue<int>("OfflineDatabases"),
            Version = reader.GetString("Version").Replace("\n", string.Empty).
                Replace("\t", string.Empty).
                Replace("Copyright (c) Microsoft Corporation", string.Empty)
        };
        if (reader.GetValue<int>("SecurityMode") == 0)
            server.SecurityMode = "Windows and SQL Server Authentication";
        else
            server.SecurityMode = "Integrated security (Windows Authentication)";
        if (isClustered)
            server.HighAvailability = "Failover Cluster";
        else
        {
            if (reader.GetValue<int>("AlwaysOnEnabled") == 1)
            {
                switch (reader.GetValue<int>("AlwaysOnManager"))
                {
                    case 0:
                        server.HighAvailability = "Always On (Pending)";
                        break;
                    case 1:
                        server.HighAvailability = "Always On (Running)";
                        break;
                    case 2:
                        server.HighAvailability = "Always On (Failed)";
                        break;
                }
            }
        }
        return server;
    }
}
