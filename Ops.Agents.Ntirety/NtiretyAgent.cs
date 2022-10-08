using Microsoft.Extensions.Logging;
using Ops.Agents.Shared.Models;
using Ops.Agents.Ntirety.Models;

namespace Ops.Agents.Ntirety;

public class NtiretyAgent : IOpsAgent
{
    readonly ILogger<NtiretyClient> _logger;
    readonly IOpsIngestApi _ingestApi;

    string[] groupsToAdd = { "BigFix" };

    public string SourceName => "Ntirety";

    public NtiretyAgent(ILogger<NtiretyClient> logger, IOpsIngestApi ingestApi)
    {
        _logger = logger;
        _ingestApi = ingestApi;
    }

    public async Task CollectAsync(AgentConfig agentConfig)
    {
        var client = new NtiretyClient(_logger, agentConfig.ApiToken);

        var activeAssets = await client.Assets.GetActiveAsync();
        var assets = new List<Asset>();
        var vms = new List<VirtualMachine>();
        var machines = new List<Machine>();

        foreach (var a in activeAssets)
        {
            try
            {
                if (a.assetType == "VMwareVM")
                {
                    var machine = NtiretyAssetToVirtualMachine(a);
                    vms.Add(machine);
                }
                else if (a.assetType == "Dedicated")
                {
                    var machine = NtiretyAssetToMachine(a);
                    machines.Add(machine);
                }
                else
                {
                    Asset asset = NtiretyAssetToBaseAsset(a);
                    assets.Add(asset);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new InvalidOperationException();
            }
        }

        await _ingestApi.IngestResource(assets);
        await _ingestApi.IngestResource(machines);
        await _ingestApi.IngestResource(vms);
    }

    private Asset NtiretyAssetToBaseAsset(NtiretyAsset a)
    {
        var assetType = a.assetType;
        if (assetType == "Unclassified")
        {
            if (string.IsNullOrEmpty(a.displayName))
            {
                assetType = "Unknown";
            }
            else if (a.displayName.Contains("IP "))
            {
                assetType = "IP Space";
            }
            else if (a.displayName.Contains("Storage"))
            {
                assetType = "Storage";
            }
            else if (a.displayName.Contains("Bandwidth"))
            {
                assetType = "Bandwidth";
            }
            else if (a.displayName.Contains("Service"))
            {
                assetType = "Service";
            }
            else
            {
                assetType = "Unknown";
            }
        }
        var asset = new Asset(a.id, SourceName, assetType)
        {
            Description = a.displayName
        };
        return asset;
    }

    private VirtualMachine NtiretyAssetToVirtualMachine(NtiretyAsset a)
    {
        var machineName = a.displayName.Replace("(" + a.name + ")", "").Trim().ToLower();
        var machine = new VirtualMachine(a.id, SourceName, machineName)
        {
            Location = a.location,
            AssetId = a.name,
            NumCpu = a.cpuCount != 0 ? a.cpuCount : null,
            OSName = a.os,
            PowerState = a.state != "unknown" ? a.state : null,
            Platform = "VMWare",
            MemoryGB = RamMbToGg(a.ram)
        };
        if (a.disks != null)
        {
            foreach (var disk in a.disks)
            {
                machine.ProvisionedSpaceGB += disk.sizeInGigabytes;
            }
        }
        AddIPAddresses(a, machine);

        return machine;
    }

    private Machine NtiretyAssetToMachine(NtiretyAsset a)
    {
        var machine = new Machine(a.id, SourceName, a.displayName)
        {
            Location = a.location,
            AssetId = a.name,
            NumCpu = a.cpuCount != 0 ? a.cpuCount : null,
            OSName = a.os,
            PowerState = a.state != "unknown" ? a.state : null,
            MemoryGB = RamMbToGg(a.ram)
        };
        AddIPAddresses(a, machine);
        return machine;
    }

    private void AddIPAddresses(NtiretyAsset a, Machine machine)
    {
        if (a.ipAddresses != null)
        {
            foreach (var ip in a.ipAddresses)
            {
                machine.IpAddress.Add(ip.ipAddress);
                foreach (var type in ip.ipTypes)
                {
                    if (groupsToAdd.Contains(type) && !machine.Group.Contains(type))
                        machine.Group.Add(type);
                }
            }
        }
    }

    private static int? RamMbToGg(string ram)
    {
        if (string.IsNullOrEmpty(ram) || ram == "0 MB")
            return null;
        return int.Parse(ram.Replace(" MB", string.Empty)) / 1024;
    }
}
