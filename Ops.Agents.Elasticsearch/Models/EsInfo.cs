using System;
namespace Ops.Agents.Elasticsearch.Models;

public class EsInfo
{
    public string name { get; set; }
    public string cluster_name { get; set; }
    public string cluster_uuid { get; set; }
    public EsVersion version { get; set; }
    public string tagline { get; set; }
}

public class EsVersion
{
    public string number { get; set; }
    public string build_hash { get; set; }
    public DateTime build_timestamp { get; set; }
    public bool build_snapshot { get; set; }
    public string lucene_version { get; set; }
}

