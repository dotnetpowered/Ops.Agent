using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json;

namespace Ops.Agents;

public abstract class Asset
{
    public Asset(string Id, string AssetType)
    {
        this.Id = Id;
        this.PartitionKey = "asset";
        this.AsOf = DateTime.Now;
        this.AssetType = AssetType;
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    [JsonProperty(PropertyName = "partitionKey")]
    public string PartitionKey { get; set; }
    public DateTime? AsOf { get; set; }
    public string AssetType { get; set; } // Machine, Cabinet, Switch, etc.
    public string? Location { get; set; } // Physical Location (City, State)

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}

public abstract class MachineResource
{
    public MachineResource(string Id, string MachineName)
    {
        this.Id = Id;
        this.PartitionKey = "asset";
        this.AsOf = DateTime.Now;

        this.MachineName = MachineName.ToLower();
        if (!this.MachineName.Contains('.'))
            this.MachineName = MachineName + ".us.xeohealth.com";
        this.PartitionKey = "machine-resource";
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    [JsonProperty(PropertyName = "partitionKey")]
    public string PartitionKey { get; set; }
    public DateTime? AsOf { get; set; }

    public string MachineName { get; set; }
}

public class Machine: Asset
{
    public Machine(string Id, string MachineName): base(Id, "Machine")
    {
        this.MachineName = MachineName.ToLower();
        if (!this.MachineName.Contains('.'))
            this.MachineName = MachineName + ".us.xeohealth.com";
        this.PartitionKey = "machine";
        this.Group = new List<string>();
        this.IpAddress = new List<string>();
        this.Tags = new ExpandoObject();
        this.AsOf = DateTime.Now;
    }

    public string MachineName { get; set; }
    public string? Description { get; set; }
    public List<string> Group { get; set; }
    public string? OSName { get; set; }
    public string? Architecture { get; set; }
    public string? AssetId { get; set; }
    public string? Platform { get; set; } // Azure, AWS, VMWare, Physical
    public List<string> IpAddress { get; set; }
    public int? NumCpu { get; set; }
    public int? MemoryGB { get; set; }
    public string? PowerState { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset? CreateDate { get; set; }
    public dynamic Tags { get; set; }
}

public class VirtualMachine: Machine
{
    public VirtualMachine(string Id, string MachineName): base(Id, MachineName)
    {
        this.PartitionKey = "virtual-machine";
    }

    public string? GuestState { get; set; }
    public string? GuestFamily { get; set; }
    public string? DrsAutomationLevel { get; set; }
    public string? VmVersion { get; set; }
    public string? VmHostId { get; set; }
    public string? VmHostName { get; set; }
    public double? ProvisionedSpaceGB { get; set; }
    public double? UsedSpaceGB { get; set; }
}

public class DeploymentTarget:Machine
{
    public DeploymentTarget(string Id, string MachineName) : base(Id, MachineName)
    {
        this.PartitionKey = "deployment-target";
    }
}

public class QueueResource : MachineResource
{
    public QueueResource(string Id, string MachineName, string QueueName) : base (Id, MachineName)
    {
        this.QueueName = QueueName;
        this.PartitionKey = "queue";
    }

    public string QueueName { get; set; }
    public string ResourceType { get; set; }
    public long? QueueSize { get; set; }
    public long? ConsumerCount { get; set; }
    public long? EnqueueCount { get; set; }
    public long? DequeueCount { get; set; }
    public long? ProducerCount { get; set; }
    public long? AverageMessageSize { get; set; }
}

public class JobExecution: MachineResource
{
    public JobExecution(string Id, string MachineName, string Description) : base(Id, MachineName)
    {
        this.Description = Description;
        this.PartitionKey = "job-execution";
    }

    public string Description { get; set; }
    public string? Status { get; set; }
    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? StopTime { get; set; }
}

public class Deployment : JobExecution
{
    public Deployment(string Id, string MachineName, string Description) : base(Id, MachineName, Description)
    {
        this.Description = Description;
        this.PartitionKey = "deployment";
    }

    public string? ReleaseId { get; set; }
    public string? ReleaseVersion { get; set; }
    public string? ProjectId { get; set; }
    public string? EnvironmentId { get; set; }
    public string? EnvironmentName { get; set; }
    public string? DeployedBy { get; set; }
}

public class UpdateAssessment : MachineResource
{
    public UpdateAssessment(string Id, string MachineName) : base(Id, MachineName)
    {
        this.PartitionKey = "update-assessment";
    }

    public DateTimeOffset? LastAccessedTime { get; set; }
    public long MissingCriticalUpdatesCount { get; set; }
    public long MissingSecurityUpdatesCount { get; set; }
    public long MissingOtherUpdatesCount { get; set; }
    public bool Compliance { get; set; }
    public DateTimeOffset? LastUpdateAgentSeenTime { get; set; }
    public string ComputerEnvironment { get; set; }
}