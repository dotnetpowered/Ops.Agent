using System;
using Refit;
using Ops.Agents.RedHat.Insights.Models;

namespace Ops.Agents.RedHat.Insights;

public interface IInsightHosts
{
    //https://console.redhat.com/api/inventory/v1/hosts

    /// <summary>
    /// Gets a list of hosts
    /// </summary>
    /// <param name="cancellationToken"></param>
    [Get("/")]
    Task<InsightsHostsResponse> GetAsync(
        CancellationToken cancellationToken = default);

    //https://console.redhat.com/api/inventory/v1/hosts/id/system_profile

    /// <summary>
    /// Gets the host detail
    /// </summary>
    /// <param name="cancellationToken"></param>
    [Get("/{id}/system_profile")]
    Task<InsightsHostResponse> GetHostAsync(
        string id,
        CancellationToken cancellationToken = default);
}

