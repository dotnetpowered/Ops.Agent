﻿using System;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Ops.Agents.RedHat.Insights.Models;

public class InsightsHostsResponse
{
    [DataMember(Name = "total")]
    public int Total { get; set; }
    [DataMember(Name = "count")]
    public int Count { get; set; }
    [DataMember(Name = "page")]
    public int page { get; set; }
    [DataMember(Name = "per_page")]
    public int PerPage { get; set; }
    [DataMember(Name = "results")]
    public InsightsHostInfo[]? Results { get; set; }
}
