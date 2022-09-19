using System;
using Newtonsoft.Json;

namespace Ops.Agents.Shared.Models;

public abstract class MachineResource
{
    public MachineResource(string Id, string Source, string MachineName)
    {
        this.Id = Id;
        this.ResourceType = "asset";
        this.AsOf = DateTime.Now;

        this.Source = Source;
        this.MachineName = MachineName.ToLower();
        if (!this.MachineName.Contains('.'))
            this.MachineName = MachineName + ".us.xeohealth.com";
        this.ResourceType = "machine-resource";
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    [JsonProperty(PropertyName = "resourceType")]
    public string ResourceType { get; set; }
    [JsonProperty(PropertyName = "source")]
    public string Source { get; set; }
    public DateTime? AsOf { get; set; }

    public string MachineName { get; set; }
}
