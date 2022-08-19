﻿using System;

namespace Ops.Agents.Shared.Models;

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