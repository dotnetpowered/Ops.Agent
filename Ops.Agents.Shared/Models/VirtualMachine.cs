﻿using System;

namespace Ops.Agents.Shared.Models;

public class VirtualMachine : Machine
{
    public VirtualMachine(string Id, string Source, string MachineName) : base(Id, Source, MachineName)
    {
        this.ResourceType = "virtual-machine";
    }

    public string? MachineType { get; set; }
    public string? GuestState { get; set; }
    public string? GuestFamily { get; set; }
    public string? DrsAutomationLevel { get; set; }
    public string? VmVersion { get; set; }
    public string? VmHostId { get; set; }
    public string? VmHostName { get; set; }
    public double? ProvisionedSpaceGB { get; set; }
    public double? UsedSpaceGB { get; set; }
}
