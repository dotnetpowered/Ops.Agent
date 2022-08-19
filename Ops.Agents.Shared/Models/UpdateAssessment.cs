﻿using System;

namespace Ops.Agents.Shared.Models;

public class UpdateAssessment : MachineResource
{
    public UpdateAssessment(string Id, string MachineName) : base(Id, MachineName)
    {
        this.PartitionKey = "update-assessment";
    }

    public DateTimeOffset? LastAccessedTime { get; set; }
    public long MissingCriticalUpdatesCount { get; set; }
    public long MissingSecurityUpdatesCount { get; set; }
    public long MissingOtherUpdatesCount { get; set; }
    public bool Compliance { get; set; }
    public DateTimeOffset? LastUpdateAgentSeenTime { get; set; }
    public string ComputerEnvironment { get; set; }
}

