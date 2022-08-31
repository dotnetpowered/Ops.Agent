﻿using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.ResourceGraph;
using Azure.ResourceManager.ResourceGraph.Models;
using Microsoft.Extensions.Logging;
using Ops.Agents.Shared.Models;
using System.Reflection;
using System.Text.Json;

namespace Ops.Agents.Azure;

public class AzureComputeAgent : IOpsAgent
{
    private readonly ILogger<AzureComputeAgent> _logger;
    private readonly IOpsIngestApi _ingestApi;

    public AzureComputeAgent(ILogger<AzureComputeAgent> logger, IOpsIngestApi ingestApi)
    {
        _logger = logger;
        _ingestApi = ingestApi;
    }

    public string SourceName => "Azure.Compute";

    public async Task CollectAsync(AgentConfig agentConfig)
    {
        var stream = this.GetType().GetTypeInfo().Assembly.GetManifestResourceStream("Ops.Agents.Azure.VirtualMachines.txt");
        if (stream == null)
            throw new InvalidOperationException("Unable to load VirtualMachines.txt resource.");
        var streamreader = new StreamReader(stream);
        var query = streamreader.ReadToEnd();
        var creds = new ClientSecretCredential(agentConfig.Tenant, agentConfig.Username, agentConfig.Password);
        var client = new ArmClient(creds);
        TenantResource tenant = client.GetTenants().FirstOrDefault();
        var qc = new QueryContent(query);
        var queryResult = tenant.Resources(qc);
        var resources = queryResult.Value.Data.ToObjectFromJson() as Object[];
        foreach (Dictionary<string,object> resource in resources)
        {
            var vm = new VirtualMachine(resource["id"].ToString(), resource["name"].ToString())
            {
                OSName = resource["offer"].ToString() + " [" + resource["publisher"].ToString() + "]",
                PowerState = resource["status"].ToString(),
                Location = "Azure " + resource["locationDisplayName"].ToString(),
                Tags = resource["tags"],
                Platform = "Azure",
                GuestFamily = resource["os"].ToString(),
                VmVersion = resource["hyperVGeneration"].ToString()
            };
            vm.IpAddress.Add(resource["privateIPAddress"].ToString());
        }

        //var client = new ArmClient(creds);
        //string resourceGroupName = agentConfig.Resource;
        //SubscriptionResource subscription = await client.GetDefaultSubscriptionAsync();
        //ResourceGroupCollection resourceGroups = subscription.GetResourceGroups();
        //ResourceGroupResource resourceGroup = await resourceGroups.GetAsync(resourceGroupName);

        //List<VirtualMachine> machines = new();
        //await foreach (VirtualMachineResource virtualMachine in resourceGroup.GetVirtualMachines())
        //{
        //    var vmInstance = await resourceGroup.GetVirtualMachineAsync(virtualMachine.Data.Name, InstanceViewType.InstanceView);

        //    var vm = new VirtualMachine(virtualMachine.Id, virtualMachine.Data.Name)
        //    {
        //        Location = "Azure " + virtualMachine.Data.Location.DisplayName,
        //        OSName = virtualMachine.Data.StorageProfile.ImageReference.Offer + " [" +
        //                 virtualMachine.Data.StorageProfile.ImageReference.Publisher + "]",
        //        GuestFamily = virtualMachine.Data.StorageProfile.OSDisk.OSType.ToString()
        //    };
        //    var v = virtualMachine.Data.InstanceView;
        //    machines.Add(vm);
        //}
        //await dbClient.UpsertItemsAsync("Metrics", "Items", items);
    }


}