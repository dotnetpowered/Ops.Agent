using System;
using Newtonsoft.Json;

namespace Ops.Agents.Shared.Models;

public abstract class MachineResource
{
    public MachineResource(string Id, string MachineName)
    {
        this.Id = Id;
        this.PartitionKey = "asset";
        this.AsOf = DateTime.Now;

        this.MachineName = MachineName.ToLower();
        if (!this.MachineName.Contains('.'))
            this.MachineName = MachineName + ".us.xeohealth.com";
        this.PartitionKey = "machine-resource";
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    [JsonProperty(PropertyName = "partitionKey")]
    public string PartitionKey { get; set; }
    public DateTime? AsOf { get; set; }

    public string MachineName { get; set; }
}
