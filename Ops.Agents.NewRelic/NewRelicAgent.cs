using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Microsoft.Extensions.Logging;
using Ops.Agents.Shared.Models;

namespace Ops.Agents.NewRelic;

public class NewRelicAgent : IOpsAgent
{
    private readonly ILogger<NewRelicAgent> _logger;
    private readonly IOpsIngestApi _ingestApi;

    public NewRelicAgent(ILogger<NewRelicAgent> logger, IOpsIngestApi ingestApi)
    {
        _logger = logger;
        _ingestApi = ingestApi;
    }

    public string SourceName => "NewRelic";

    public async Task CollectAsync(AgentConfig agentConfig)
    {
        var graphQLClient = new GraphQLHttpClient(
            "https://api.newrelic.com/graphql",
            new SystemTextJsonSerializer());
        graphQLClient.HttpClient.DefaultRequestHeaders.Add(
            "API-Key", agentConfig.ApiToken);
        var request = new GraphQLRequest {
                Query = @"{
                            actor {
                            entitySearch(queryBuilder: {type: HOST}) {
                                query
                                results {
                                entities {
                                    name
                                    entityType
                                    guid
                                    tags {
                                        key
                                        values
                                    }
                                }
                                }
                            }
                        }
                    }"
        };
        var graphQLResponse = await graphQLClient.SendQueryAsync<Response>(request);

        var machines = new List<Machine>();
        foreach (var item in graphQLResponse.Data.actor.entitySearch.results.entities)
        {
            var fullName = item.GetTagValue("fullHostname");
            var machine = new Machine(item.guid, fullName)
            {
                NumCpu = item.GetTagValueInt("processorCount"),
                MemoryGB = (int) (item.GetTagValueDouble("systemMemoryBytes") / 1024 / 1024),
                PowerState = item.GetTagValue("hostStatus"),
                Platform = item.GetTagValue("instanceType"),
                OSName = item.GetTagValue("windowsPlatform") ??
                    item.GetTagValue("linuxDistribution") + " " +
                    item.GetTagValue("kernelVersion"),
                Permalink = item.GetTagValue("permalink"),
                AgentVersion = item.GetTagValue("agentName") + " " +
                    item.GetTagValue("agentVersion")
            };
            machines.Add(machine);
        }
    }
}

