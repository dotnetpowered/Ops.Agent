﻿using System;
using System.Dynamic;

namespace Ops.Agents.Shared.Models;

public class Machine : Asset
{
    public Machine(string Id, string MachineName) : base(Id, "Machine")
    {
        this.MachineName = MachineName.ToLower();
        if (!this.MachineName.Contains('.'))
            this.MachineName = MachineName + ".us.xeohealth.com";
        this.PartitionKey = "machine";
        this.Group = new List<string>();
        this.IpAddress = new List<string>();
        this.Tags = new ExpandoObject();
        this.AsOf = DateTime.Now;
    }

    public string MachineName { get; set; }
    public string? Description { get; set; }
    public List<string> Group { get; set; }
    public string? OSName { get; set; }
    public string? Architecture { get; set; }
    public string? AssetId { get; set; }
    public string? Platform { get; set; } // Azure, AWS, VMWare, Physical
    public List<string> IpAddress { get; set; }
    public int? NumCpu { get; set; }
    public int? MemoryGB { get; set; }
    public string? PowerState { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset? CreateDate { get; set; }
    public dynamic Tags { get; set; }
}