using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Refit;

namespace Ops.Agents.RedHat.Insights;

public class InsightsClient
{
    private readonly ILogger _logger;
    private readonly HttpClient _httpClient;


    public InsightsClient(ILogger logger, string username, string password)
    {
        _logger = logger;
        var authenticationString = $"{username}:{password}";
        var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));

        _httpClient = new HttpClient() {
            BaseAddress = new Uri("https://console.redhat.com/api/inventory/v1/hosts")
        };
        var refitSettings = new RefitSettings
        {
            ContentSerializer = new NewtonsoftJsonContentSerializer(
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                    //, Converters = { new StringEnumConverter() }
                }
            )
        };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("basic", base64EncodedAuthenticationString);
        Hosts = RestService.For<IInsightHosts>(_httpClient, refitSettings);
    }

    /// <summary>
    /// InsightHosts
    /// </summary>
    public IInsightHosts Hosts { get; }

}

