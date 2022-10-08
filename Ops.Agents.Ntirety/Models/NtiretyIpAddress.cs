using System;

namespace Ops.Agents.Ntirety.Models;

public class NtiretyIpAddress
{
    public string displayName { get; set; }
    public string ipAddress { get; set; }
    public string netMask { get; set; }
    public string vlan { get; set; }
    public DateTime updatedUtc { get; set; }
    public List<string> ipTypes { get; set; }
}
