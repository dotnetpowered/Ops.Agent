using System;
using Newtonsoft.Json;

namespace Ops.Agents.Shared.Models;

public abstract class Asset
{
    public Asset(string Id, string AssetType)
    {
        this.Id = Id;
        this.PartitionKey = "asset";
        this.AsOf = DateTime.Now;
        this.AssetType = AssetType;
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    [JsonProperty(PropertyName = "partitionKey")]
    public string PartitionKey { get; set; }
    public DateTime? AsOf { get; set; }
    public string AssetType { get; set; } // Machine, Cabinet, Switch, etc.
    public string? Location { get; set; } // Physical Location (City, State)

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}