using System;

namespace Ops.Agents.Shared.Models;

public class DeploymentTarget : Machine
{
    public DeploymentTarget(string Id, string MachineName) : base(Id, MachineName)
    {
        this.PartitionKey = "deployment-target";
    }
}
