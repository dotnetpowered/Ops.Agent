using System;

namespace Ops.Agents.Shared.Models;

public class QueueResource : MachineResource
{
    public QueueResource(string Id, string Source, string MachineName, string QueueName) : base(Id, Source, MachineName)
    {
        this.QueueName = QueueName;
        this.ResourceType = "queue";
    }

    public string QueueName { get; set; }
    public long? QueueSize { get; set; }
    public long? ConsumerCount { get; set; }
    public long? EnqueueCount { get; set; }
    public long? DequeueCount { get; set; }
    public long? ProducerCount { get; set; }
    public long? AverageMessageSize { get; set; }
}

