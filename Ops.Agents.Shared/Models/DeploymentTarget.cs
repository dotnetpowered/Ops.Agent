using System;

namespace Ops.Agents.Shared.Models;

public class DeploymentTarget : Machine
{
    public DeploymentTarget(string Id, string Source, string MachineName) : base(Id, Source, MachineName)
    {
        this.ResourceType = "deployment-target";
    }
}
