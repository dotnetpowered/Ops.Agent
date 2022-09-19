using System;

namespace Ops.Agents.Shared.Models;

public class JobExecution : MachineResource
{
    public JobExecution(string Id, string Source, string MachineName, string Description) : base(Id, Source, MachineName)
    {
        this.Description = Description;
        this.ResourceType = "job-execution";
    }

    public string Description { get; set; }
    public string? Status { get; set; }
    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? StopTime { get; set; }
}
