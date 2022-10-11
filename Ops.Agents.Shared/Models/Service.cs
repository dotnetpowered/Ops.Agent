using System;

namespace Ops.Agents.Shared.Models;

public class Service : MachineResource
{
    public Service(string Id, string Source,
                   string MachineName, string ServiceType,
                   string Description) : base(Id, Source, MachineName)
    {
        this.Description = Description;
        this.ResourceType = "service";
        this.ServiceType = ServiceType;
    }

    public string? Description { get; set; }
    public string? ServiceType { get; set; }
    public string? InstanceName { get; set; }
    public string? ClusterName { get; set; }
    public string? Language { get; set; }
    public string? Version { get; set; }
    public string? SecurityMode { get; set; }
    public string? HighAvailability { get; set; }
    public long? MemoryUsageMB { get; set; }
    public DateTime? ServiceStartTime { get; set; }
    public bool? ActiveNode { get; set; }
    public string? Status { get; set; }

    // Machine-level attributes discoverd by this service
    public int? NumCpu { get; set; }
    public int? MemoryGB { get; set; }
    public string? OSName { get; set; }
    public string? Architecture { get; set; }


    //public long? Blocks { get; set; }
    //public bool? MemoryPressure { get; set; }
    //public string? AgentStatus { get; set; }
}
