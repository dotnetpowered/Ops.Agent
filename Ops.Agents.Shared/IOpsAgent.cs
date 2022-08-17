using System;
namespace Ops.Agents;

public interface IOpsAgent
{
    public Task CollectAsync(AgentConfig agentConfig);
    public string SourceName { get; }
}

