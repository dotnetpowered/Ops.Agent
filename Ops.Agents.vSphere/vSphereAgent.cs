using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Ops.Agents.Shared;
using Ops.Agents.Shared.Models;

namespace Ops.Agents.vSphere;

public class vSphereAgent : IOpsAgent
{
    private readonly ILogger<vSphereAgent> _logger;
    private readonly IOpsIngestApi _ingestApi;

    public vSphereAgent(ILogger<vSphereAgent> logger, IOpsIngestApi ingestApi)
    {
        _logger = logger;
        _ingestApi = ingestApi;
    }

    public string SourceName => "VMWare.vSphere";

    public async Task CollectAsync(AgentConfig agentConfig)
    {
        PowerShell ps = PowerShell.Create();
        //ps.Runspace.SessionStateProxy.SetVariable("a", new int[] { 1, 2, 3 });
        //ps.AddScript("$a");
        //ps.AddCommand("foreach-object");
        //ps.AddParameter("process", ScriptBlock.Create("$_ * 2"));

        // Load query from embedded resource
        string query = ResourceUtils.LoadEmbeddedResource<vSphereAgent>("vmware_serverlist.ps1");
        query = query.Replace("{{USER_NAME}}", agentConfig.Username);
        query = query.Replace("{{PASSWORD}}", agentConfig.Password);
        query = query.Replace("{{SERVER}}", agentConfig.Server);
        ps.AddScript(query);

        Collection<PSObject> results = ps.Invoke();
        if (ps.HadErrors)
        {
            foreach (var err in ps.Streams.Error)
            {
                Console.WriteLine(err.Exception);
            }
        }
        PSObject result = results.Skip(1).First();
        var obj = JsonSerializer.Deserialize<JsonElement>(result.ToString());
        //Name, NumCpu, MemoryGB, PowerState, Id, Notes, ProvisionedSpaceGB, UsedSpaceGB, CreateDate, DrsAutomationLevel
        //@{ Name = 'OSName'; Expression ={$_.Guest.OSFullName}; }, `
        //@{ Name = 'IpAddress'; Expression ={$_.Guest.IPAddress}; }, `
        //@{ Name = 'HostName'; Expression ={$_.Guest.HostName}; }, `
        //@{ Name = 'GuestState'; Expression ={$_.Guest.State}; }, `
        //@{ Name = 'GuestFamily'; Expression ={$_.Guest.GuestFamily}; }, `
        //@{ Name = 'VmVersion'; Expression ={$_.Version}; }, `
        //@{ Name = 'VmHostId'; Expression ={$_.VMHost.Id}; }, `
        //@{ Name = 'VmHostName'; Expression ={$_.VMHost.Name}; } `
        var machines = from element in obj.EnumerateArray()
                       where GetString(element, "HostName")!=null
                       select new VirtualMachine(
                                    GetString(element, "Id"),
                                    this.SourceName,
                                    GetString(element, "HostName"))
                       {
                           Description = GetString(element, "Name"),
                           NumCpu = GetInt(element, "NumCpu"),
                           MemoryGB = ((int?)GetDouble(element, "MemoryGB")),
                           PowerState = GetIntAsString(element, "PowerState"),
                           ProvisionedSpaceGB = GetDouble(element, "ProvisionedSpaceGB"),
                           UsedSpaceGB = GetDouble(element, "UsedSpaceGB"),
                           Notes = GetString(element, "Notes"),
                           OSName = GetString(element, "OSName"),
                           VmHostId = GetString(element, "VmHostId"),
                           VmVersion = GetIntAsString(element, "VmVersion"),
                           VmHostName = GetString(element, "VmHostName"),
                           DrsAutomationLevel = GetIntAsString(element, "DrsAutomationLevel"),
                           GuestFamily = GetString(element, "GuestFamily"),
                           GuestState = GetIntAsString(element, "GuestState"),
                           CreateDate = GetStringAsDateTime(element, "CreateDate")
                       };
        await _ingestApi.IngestResource(machines);
    }

    string GetHostName(JsonElement element)
    {
        var hostname = GetString(element, "HostName");
        if (string.IsNullOrEmpty(hostname))
            return "missing";
        else
            return hostname;            
    }

    string? GetString(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out JsonElement prop))
            return prop.GetString();
        else
            return null;
    }

    int? GetInt(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out JsonElement prop))
            return prop.GetInt32();
        else
            return null;
    }

    double? GetDouble(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out JsonElement prop))
            return prop.GetDouble();
        else
            return null;
    }

    string? GetIntAsString(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out JsonElement prop))
            return prop.GetInt32().ToString();
        else
            return null;
    }

    DateTimeOffset? GetStringAsDateTime(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out JsonElement prop))
            return DateTimeOffset.Parse(prop.GetString());
        else
            return null;
    }

}

