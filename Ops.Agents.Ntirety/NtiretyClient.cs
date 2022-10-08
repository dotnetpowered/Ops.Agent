using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Refit;

namespace Ops.Agents.Ntirety;

public class NtiretyClient
{
    private readonly ILogger _logger;
    private readonly HttpClient _httpClient;

    public NtiretyClient(ILogger<NtiretyClient> logger, string apiKey)
    {
        _logger = logger;

        _httpClient = new HttpClient() {
            BaseAddress = new Uri("https://api.ntirety.net/")            
        };
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
        _httpClient.Timeout = new TimeSpan(0, 2, 0);
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
        Assets = RestService.For<INtiretyAssets>(_httpClient, refitSettings);
    }

    /// <summary>
    /// INtiretyAssets
    /// </summary>
    public INtiretyAssets Assets { get; }

}

