using System;

namespace Ops.Agents.Ntirety.Models;

public class NtiretyAsset
{
    public string id { get; set; }
    public int accountNumber { get; set; }
    public int subId { get; set; }
    public string name { get; set; }
    public string displayName { get; set; }
    public string description { get; set; }
    public string assetType { get; set; }
    public string location { get; set; }
    public int cpuCount { get; set; }
    public string ram { get; set; }
    public string os { get; set; }
    public string state { get; set; }
    public List<NtiretyDisk> disks { get; set; }
    public List<NtiretyIpAddress> ipAddresses { get; set; }
    public List<NtiretyTag> tags { get; set; }
}
