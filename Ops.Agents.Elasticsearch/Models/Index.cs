using System;
using System.Text.Json.Serialization;

namespace Ops.Agents.Elasticsearch.Models;

public class EsIndex
{
    public string health { get; set; }
    public string status { get; set; }
    public string index { get; set; }
    public string pri { get; set; }
    public string rep { get; set; }

    [JsonPropertyName("docs.count")]
    public string docscount { get; set; }

    [JsonPropertyName("docs.deleted")]
    public string docsdeleted { get; set; }

    [JsonPropertyName("store.size")]
    public string storesize { get; set; }

    [JsonPropertyName("pri.store.size")]
    public string pristoresize { get; set; }
}