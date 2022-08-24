using System;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Ops.Agents.RedHat.Insights.Models;

[DataContract]
public class InsightsSystemProfile
{
    [DataMember(Name = "os_release")]
    public string? OsRelease { get; set; }
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
    public long? SystemMemoryBytes { get; set; }
}
