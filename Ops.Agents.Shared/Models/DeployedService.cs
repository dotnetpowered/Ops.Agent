using System;

namespace Ops.Agents.Shared.Models;

public class DeployedService : Service
{
    public DeployedService(string Id, string Source, string MachineName, string Description) : base(Id, Source, MachineName, "Deployed", Description)
    {
        this.Description = Description;
    }

    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? StopTime { get; set; }
    public string? DeploymentId { get; set; }
    public string? Environment { get; set; }
    public string? DeployedBy { get; set; }
}
