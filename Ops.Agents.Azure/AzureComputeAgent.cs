using Azure;
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
using Ops.Agents.Shared;

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
        string query = ResourceUtils.LoadEmbeddedResource<AzureUpdateAgent>("VirtualMachines.txt");
        var creds = new ClientSecretCredential(agentConfig.Tenant, agentConfig.Username, agentConfig.Password);
        var client = new ArmClient(creds);
        TenantResource tenant = client.GetTenants().FirstOrDefault();
        var qc = new QueryContent(query);
        var queryResult = tenant.Resources(qc);
        var resources = queryResult.Value.Data.ToObjectFromJson() as Object[];

        List<VirtualMachine> virtualMachines = new();
        foreach (Dictionary<string,object> resource in resources)
        {
            var vm = new VirtualMachine(resource["id"].ToString(), this.SourceName, resource["name"].ToString())
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
            virtualMachines.Add(vm);
        }

        await _ingestApi.IngestResource(virtualMachines);


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