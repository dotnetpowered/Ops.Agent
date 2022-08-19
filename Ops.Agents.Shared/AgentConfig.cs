using System;
namespace Ops.Agents;

public class AgentConfig
{
    public string? Agent { get; set; }
    public string? Server { get; set; }
	public string? Url { get; set; }
	public string? WorkspaceId { get; set; }
    public string? Username { get; set; }
	public string? Password { get; set; }
	public int?    Port { get; set; }
	public string? ApiToken { get; set; }
}
