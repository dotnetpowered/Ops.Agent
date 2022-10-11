using System;

namespace Ops.Agents.Shared.Models;

public class JobScheduler : MachineResource
{
    public JobScheduler(string Id, string Source, string MachineName) : base(Id, Source, MachineName)
    {
        this.ResourceType = "job-scheduler";
    }
}
