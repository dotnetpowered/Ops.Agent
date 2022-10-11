using System;

namespace Ops.Agents.Shared.Models;

public class Job : MachineResource
{
    public Job(string Id, string Source, string MachineName, string Description) : base(Id, Source, MachineName)
    {
        this.Description = Description;
        this.ResourceType = "job-execution";
    }

    public string Description { get; set; }
}
