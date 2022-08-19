using System;

namespace Ops.Agents.Shared.Models;

public class JobExecution : MachineResource
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
