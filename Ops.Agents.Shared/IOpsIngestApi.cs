using System;
using Refit;

namespace Ops.Agents;

public interface IOpsIngestApi
{
    [Post("")]
    Task<IngestResult> UpsertResource([Body] object resource);
}

public class IngestResult
{
    public string? Result;
}
