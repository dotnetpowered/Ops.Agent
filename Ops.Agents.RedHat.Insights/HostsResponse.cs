using System;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Ops.Agents.RedHat.Insights;

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

public class InsightsHostResponse
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


public class InsightsHost
{
    [DataMember(Name = "id")]
    public string? Id { get; set; }
    [DataMember(Name = "}")]
    public InsightsSystemProfile? SystemProfile { get; set; }
}

public class InsightsSystemProfile
{
    [DataMember(Name = "os_release")]
    public string? OsReleases { get; set; }
    [DataMember(Name = "infrastructure_type")]
    public string? InfrastructureType { get; set; }
    [DataMember(Name = "infrastructure_vendor")]
    public string? InfrastructureVendor { get; set; }
    [DataMember(Name = "cores_per_socket")]
    public int? CoresPerSocket { get; set; }
    [DataMember(Name = "number_of_sockets")]
    public int? NumberOfSockets { get; set; }
    [DataMember(Name = "cpu_model")]
    public string? CpuModel { get; set; }
    [DataMember(Name = "arch")]
    public string? Architecture { get; set; }
    [DataMember(Name = "os_kernel_version")]
    public string? OsKernelVersion { get; set; }
    [DataMember(Name = "system_memory_bytes")]
    public int? SystemMemoryBytes { get; set; }
}
