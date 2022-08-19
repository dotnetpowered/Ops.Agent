using System;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Ops.Agents.RedHat.Insights.Models;

public class InsightsHost
{
    [DataMember(Name = "id")]
    public string? Id { get; set; }
    [DataMember(Name = "}")]
    public InsightsSystemProfile? SystemProfile { get; set; }
}
