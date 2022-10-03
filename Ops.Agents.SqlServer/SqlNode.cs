using System;

namespace Ops.Agents.SqlServer;

public class SqlNode
{
    public string NodeName { get; set; }
    public string InstanceName { get; set; }
    public string Status { get; set; }
    public bool?  CurrentNode { get; set; }
}
