using System;
using Microsoft.Extensions.Logging;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Compute.V1;
using Ops.Agents.Shared.Models;

namespace Ops.Agents.Google.Cloud;

public class GoogleComputeAgent : IOpsAgent
{
    private readonly ILogger<GoogleComputeAgent> _logger;
    private readonly IOpsIngestApi _ingestApi;

    public GoogleComputeAgent(ILogger<GoogleComputeAgent> logger, IOpsIngestApi ingestApi)
    {
        _logger = logger;
        _ingestApi = ingestApi;
    }

    public string SourceName => "Google.Compute";

    public async Task CollectAsync(AgentConfig agentConfig)
    {
       
        var builder = new InstancesClientBuilder();
        var creds = GoogleCredential.FromFile(agentConfig.ApiToken);
        builder.GoogleCredential = creds;
        var client = builder.Build();

        // Initialize client that will be used to send requests. This client only needs to be created
        // once, and can be reused for multiple requests.
        IList<Instance> allInstances = new List<Instance>();

        // Make the request to list all VM instances in a project.
        List<VirtualMachine> virtualMachines = new();
        await foreach (var instancesByZone in client.AggregatedListAsync(agentConfig.Project))
        {
            // The result contains a KeyValuePair collection, where the key is a zone and the value
            // is a collection of instances in that zone.
            Console.WriteLine($"Instances for zone: {instancesByZone.Key}");
            foreach (var instance in instancesByZone.Value.Instances)
            {
                var vm = new VirtualMachine(instance.Id.ToString(), this.SourceName, instance.Name)
                {
                    Platform = "GCP",
                    CreateDate = DateTime.Parse(instance.CreationTimestamp),
                    PowerState = instance.Status,
                    Location = "GCP " + instancesByZone.Key.Split('/')[1], // zones/us-east1-b
                    Architecture = instance.Disks.First().Architecture // X86_64
                };

                // MachineType
                // https://www.googleapis.com/compute/v1/projects/phonic-adviser-363000/zones/us-east1-b/machineTypes/e2-micro
                var machineType = instance.MachineType.Split('/');
                vm.MachineType = machineType[machineType.Length - 1];

                // OSName
                // https://www.googleapis.com/compute/v1/projects/debian-cloud/global/licenses/debian-11-bullseye
                var license = instance.Disks.First().Licenses.First().Split('/');
                vm.OSName = license[license.Length - 1];

                vm.IpAddress = (from net in instance.NetworkInterfaces
                                select net.NetworkIP).ToList();

                vm.Tags = instance.Tags.Items;
                // Tags
                // Labels

                virtualMachines.Add(vm);
            }
        }

        await _ingestApi.UpsertResource(virtualMachines);
    }

}
