using Azure;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Resources;
using Microsoft.Extensions.Logging;
using Ops.Agents.Shared.Models;

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
        var creds = new ClientSecretCredential(agentConfig.Tenant, agentConfig.Username, agentConfig.Password);
        var client = new ArmClient(creds);
        string resourceGroupName = "myResourceGroup";
        SubscriptionResource subscription = await client.GetDefaultSubscriptionAsync();
        ResourceGroupCollection resourceGroups = subscription.GetResourceGroups();
        ResourceGroupResource resourceGroup = await resourceGroups.GetAsync(resourceGroupName);

        await foreach (VirtualMachineResource virtualMachine in resourceGroup.GetVirtualMachines())
        {
            var vm = new VirtualMachine(virtualMachine.Id, virtualMachine.Data.Name)
            {
            };
        }
        //await dbClient.UpsertItemsAsync("Metrics", "Items", items);
    }


}