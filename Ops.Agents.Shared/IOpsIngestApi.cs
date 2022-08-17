using System;
using Refit;

namespace Ops.Agents;

public interface IOpsIngestApi
{
    [Post("/api/OpsApiIngest")]
    Task<IngestResult> UpsertResource(string source, string code, [Body] string resourceJson);
}

public class IngestResult
{
    public string? Result;
}
