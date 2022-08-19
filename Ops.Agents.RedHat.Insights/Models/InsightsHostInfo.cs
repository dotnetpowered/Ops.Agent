using System;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Ops.Agents.RedHat.Insights.Models;

public class InsightsHostInfo
{
    [DataMember(Name = "display_name")]
    public string? DisplayName { get; set; }
    [DataMember(Name = "fqdn")]
    public string? Fqdn { get; set; }
    [DataMember(Name = "id")]
    public string? Id { get; set; }
    [DataMember(Name = "created")]
    public DateTime? Created { get; set; }
    [DataMember(Name = "updated")]
    public DateTime? Updated { get; set; }
}
