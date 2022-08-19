using System;

namespace Ops.Agents.Shared.Models;

public class QueueResource : MachineResource
{
    public QueueResource(string Id, string MachineName, string QueueName) : base(Id, MachineName)
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

