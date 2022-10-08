using System;
using Refit;
using Ops.Agents.Ntirety.Models;

namespace Ops.Agents.Ntirety;

public interface INtiretyAssets
{
    /// <summary>
    /// Gets a list of active assets
    /// </summary>
    /// <param name="cancellationToken"></param>
    [Get("/assets/v1/active")]
    Task<List<NtiretyAsset>> GetActiveAsync(
        CancellationToken cancellationToken = default);
}

