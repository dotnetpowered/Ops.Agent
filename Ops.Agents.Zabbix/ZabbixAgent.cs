using System;
using System.Dynamic;
using System.Linq;
using Microsoft.Extensions.Logging;
using ZabbixApi;
using ZabbixApi.Entities;
using ZabbixApi.Services;
using Ops.Agents.Shared.Models;

namespace Ops.Agents.Zabbix;

public class ZabbixAgent : IOpsAgent
{
    private readonly ILogger<ZabbixAgent> _logger;
    private readonly IOpsIngestApi _ingestApi;

    public ZabbixAgent(ILogger<ZabbixAgent> logger, IOpsIngestApi ingestApi)
    {
        _logger = logger;
        _ingestApi = ingestApi;
    }

    public string SourceName => "Zabbix";

    public async Task CollectAsync(AgentConfig agentConfig)
    {
        using (var context = new Context(agentConfig.Url, agentConfig.Username, agentConfig.Password))
        {
            HostInclude[] hostInclude = new[] { HostInclude.Groups, HostInclude.Inventory, HostInclude.Items, HostInclude.Applications };
            var p = new Dictionary<string, object>();
            p.Add("output", "extend");// new[] { "name", "description", "host", "status", "lastaccess", "items" });
            //p.Add("selectInventory", "extend");
            //p.Add("selectTags", "extend");
            //p.Add("selectItems", "extend");
            //p.Add("inheritedTags", "true");
            var hosts = context.Hosts.Get(null, hostInclude, p);

            var machines = from h in hosts
                           select new Machine(h.Id, h.host)
                           {
                               Group = (from g in h.groups select g.name).ToList(),
                               PowerState = h.status.ToString(),
                               OSName = GetOS(h),
                               MemoryGB = GetMemGB(h),
                               Architecture = GetArchitecture(h),
                               Tags = GetTags(h)
                           };
           // await dbClient.UpsertItemsAsync("Metrics", "Items", machines);
        }
    }

    dynamic GetTags(ZabbixApi.Entities.Host host)
    {
        var tags = new ExpandoObject();
        foreach (var item in host.items)
        {
            if (item.key_ != "system.uname" &&
                item.key_!= "agent.hostname" &&
                !item.key_.StartsWith("net.if.in") &&
                !item.key_.StartsWith("net.if.out") &&
                !item.key_.Contains("{$URL},{HOST.HOST}"))
            {
                switch (item.value_type) {
                    case Item.ValueType.NumericFloat:
                        tags.TryAdd(item.key_, double.Parse(item.lastvalue));
                        break;
                    case Item.ValueType.NumericUnsigned:
                        tags.TryAdd(item.key_, ulong.Parse(item.lastvalue));
                        break;
                    default:
                        tags.TryAdd(item.key_, item.lastvalue);
                        break;

                }
            }
                
        }
        tags.TryAdd("applications", (from app in host.applications
                                     select app.name));
        return tags;
    }

    int GetMemGB(ZabbixApi.Entities.Host host)
    {
        var value = (from i in host.items
                         where i.key_ == "vm.memory.size[total]"
                         select i.lastvalue).FirstOrDefault();
        long.TryParse(value, out long mem);
        var memGB = (int)Math.Ceiling(mem / 1024.0 / 1024.0 / 1024.0);
        return memGB;
    }

    string? GetOS(ZabbixApi.Entities.Host host)
    {
        //Example:
        //Windows <host> 10.0.14393 Microsoft Windows Server 2016 Standard x64
        //Linux <host> 3.10.0-957.1.3.el7.x86_64 #1 SMP Thu Nov 15 17:36:42 UTC 2018 x86_64
        var value = (from i in host.items
                     where i.key_ == "system.uname"
                     select i.lastvalue).FirstOrDefault();
        string os;
        if (value != null)
        {
            var parts = new List<string>(value.Split(' '));
            if (parts[0] == "Windows")
            {
                parts.RemoveRange(0, 3);
                parts.RemoveAt(parts.Count - 1);
            }
            if (parts[0] == "Linux")
            {
                parts.RemoveAt(1);
                parts.RemoveRange(parts.Count - 7, 7);
            }
            os = String.Join(' ', parts);
        }
        else
        {
            os = host.inventory.os;
        }
        return os;
    }

    string? GetArchitecture(ZabbixApi.Entities.Host host)
    {
        //Example:
        //Windows <host> 10.0.14393 Microsoft Windows Server 2016 Standard x64
        //Linux <host> 3.10.0-957.1.3.el7.x86_64 #1 SMP Thu Nov 15 17:36:42 UTC 2018 x86_64
        var value = (from i in host.items
                         where i.key_ == "system.uname"
                         select i.lastvalue).FirstOrDefault();
        if (value == null)
            return null;
        var parts = value.Split(' ');
        return parts.Last();
    }
}

