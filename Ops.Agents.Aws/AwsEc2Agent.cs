using Amazon;
using Amazon.EC2;
using Amazon.Runtime;
using Microsoft.Extensions.Logging;
using Ops.Agents.Shared.Models;

namespace Ops.Agents.Aws;


public class AwsEc2Agent : IOpsAgent
{
    private readonly ILogger<AwsEc2Agent> _logger;
    private readonly IOpsIngestApi _ingestApi;

    public AwsEc2Agent(ILogger<AwsEc2Agent> logger, IOpsIngestApi ingestApi)
    {
        _logger = logger;
        _ingestApi = ingestApi;
    }

    public string SourceName => "Aws.Ec2";

    public async Task CollectAsync(AgentConfig agentConfig)
    {
        using AmazonEC2Client client = new AmazonEC2Client(
            new BasicAWSCredentials(agentConfig.Username, agentConfig.Password),
            RegionEndpoint.GetBySystemName(agentConfig.Region));
        var instances = await client.DescribeInstancesAsync();

        List<VirtualMachine> virtualMachines = new();
        foreach (var reservation in instances.Reservations)
        {
            foreach (var instance in reservation.Instances)
            {
                var vm = new VirtualMachine(instance.VpcId, this.SourceName, instance.PrivateDnsName)
                {
                    Architecture = instance.Architecture.ToString(),
                    VmVersion = instance.Hypervisor.Value,
                    Platform = "AWS",
                    PowerState = instance.State.Name.Value,
                    GuestFamily = instance.Platform.Value,
                    NumCpu = instance.CpuOptions.CoreCount,
                    Location = "AWS " + agentConfig.Region
                };
                vm.IpAddress.Add(instance.PrivateIpAddress);
                if (instance.PublicIpAddress != null)
                    vm.IpAddress.Add(instance.PublicIpAddress);
                virtualMachines.Add(vm);
            }
        }

        await _ingestApi.UpsertResource(virtualMachines);
    }
}