using System;

namespace Ops.Agents.Shared.Models;

public abstract class MachineResource
{
    public MachineResource(string Id, string Source, string MachineName)
    {
        this.Id = Id;
        this.AsOf = DateTime.Now;

        this.Source = Source;
        this.MachineName = MachineName.ToLower();
        this.ResourceType = "machine-resource";
    }

    public string Id { get; set; }
    public string ResourceType { get; set; }
    public string Source { get; set; }
    public DateTime? AsOf { get; set; }

    public string MachineName { get; set; }
}
