using System;

namespace Ops.Agents.Shared.Models;

public class VirtualMachine : Machine
{
    public VirtualMachine(string Id, string MachineName) : base(Id, MachineName)
    {
        this.PartitionKey = "virtual-machine";
    }

    public string? GuestState { get; set; }
    public string? GuestFamily { get; set; }
    public string? DrsAutomationLevel { get; set; }
    public string? VmVersion { get; set; }
    public string? VmHostId { get; set; }
    public string? VmHostName { get; set; }
    public double? ProvisionedSpaceGB { get; set; }
    public double? UsedSpaceGB { get; set; }
}
