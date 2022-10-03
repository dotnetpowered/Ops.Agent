using System;
using Refit;

namespace Ops.Agents;

public interface IOpsIngestApi
{
    [Post("/OpsApiIngest")]
    Task<IngestResult> IngestResource([Body] IEnumerable<object> resources);
}

public class IngestResult
{
    public string? Result;
}
