using System;
namespace Ops.Agents;

public class AgentConfig
{
    public string? Agent { get; set; }
    public string? Server { get; set; }
	public string? Url { get; set; }
    public string? Tenant { get; set; }
    public string? Region { get; set; }
    public string? Project { get; set; }
    public string? Resource { get; set; }
    public string? Username { get; set; }
	public string? Password { get; set; }
	public int?    Port { get; set; }
	public string? ApiToken { get; set; }
    public string? ConnectionString { get; set; }
}
