using System;

namespace Ops.Agents.Shared.Models;

public class DataServer: MachineResource
{
    public DataServer(string Id, string Source, string MachineName) : base(Id, Source, MachineName)
    {
        this.ResourceType = "data-server";
    }

    public int? NumCpu { get; set; }
    public int? MemoryGB { get; set; }

    public string? InstanceName { get; set; }
    public string? ClusterName { get; set; }
    public string? Language { get; set; }
    public string? Version { get; set; }
    public string? SecurityMode { get; set; }
    public string? HighAvailability { get; set; }
    public long? MemoryUsageMB { get; set; }
    public long? OnlineRespositories { get; set; }
    public long? OfflineRespositories { get; set; }
    public DateTime? ServerStartTime { get; set; }
    public bool? ActiveNode { get; set; }
    public string? Status { get; set; }

    //public long? Blocks { get; set; }
    //public bool? MemoryPressure { get; set; }
    //public string? AgentStatus { get; set; }
}
