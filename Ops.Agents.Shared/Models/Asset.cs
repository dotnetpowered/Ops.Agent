using System;
using Newtonsoft.Json;

namespace Ops.Agents.Shared.Models;

public class Asset
{
    public Asset(string Id, string Source, string AssetType)
    {
        this.Id = Id;
        this.ResourceType = "asset";
        this.AsOf = DateTime.Now;
        this.AssetType = AssetType;
        this.Source = Source;
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    [JsonProperty(PropertyName = "resourceType")]
    public string ResourceType { get; set; }
    [JsonProperty(PropertyName = "source")]
    public string Source { get; set; }
    public DateTime? AsOf { get; set; }
    public string AssetType { get; set; } // Machine, Cabinet, Switch, etc.
    public string? Location { get; set; } // Physical Location (City, State)
    public string? Permalink { get; set; }
    public string? AgentVersion { get; set; }
    public string? Description { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}