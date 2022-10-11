using System;

namespace Ops.Agents.Shared.Models;

public class DataService: Service
{
    public DataService(string Id, string Source,
                       string MachineName, string DataServiceType,
                       string Description) : base(Id, Source, MachineName, "Data", Description)
    {
        this.DataServiceType = DataServiceType;
    }

    public string DataServiceType { get; set; }
    public long? OnlineRespositories { get; set; }
    public long? OfflineRespositories { get; set; }

    //public long? Blocks { get; set; }
    //public bool? MemoryPressure { get; set; }
    //public string? AgentStatus { get; set; }
}
