﻿using System.Text.Json;
using Microsoft.Extensions.Logging;
using Ops.Agents.Shared.Models;

namespace Ops.Agents.Amq;

public class AmqAgent : IOpsAgent
{
    private readonly ILogger<AmqAgent> _logger;
    private readonly IOpsIngestApi _ingestApi;

    public AmqAgent(ILogger<AmqAgent> logger, IOpsIngestApi ingestApi)
    {
        _logger = logger;
        _ingestApi = ingestApi;
    }

    public string SourceName => "AcitveMQ";

    public async Task CollectAsync(AgentConfig agentConfig)
    {
        // http://host:port/api/jolokia/read/org.apache.activemq:type=Broker,brokerName=localhost
        var urlPrefix = $"http://{agentConfig.Server}:{agentConfig.Port}";
        Uri uri = new Uri(urlPrefix  + "/api/jolokia/read/org.apache.activemq:type=Broker,brokerName=localhost");

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "amq-agent");
        var authenticationString = $"{agentConfig.Username}:{agentConfig.Password}";
        var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.UTF8.GetBytes(authenticationString));
        client.DefaultRequestHeaders.Add("Authorization", "Basic " + base64EncodedAuthenticationString);
        var content = await client.GetStringAsync(uri);
        var json = JsonDocument.Parse(content);

        var brokerJson = json.RootElement.GetProperty("value");

        var list = brokerJson.GetProperty("Queues").EnumerateArray();
        var BrokerVersion = brokerJson.GetProperty("BrokerVersion").GetString();
        var UptimeSec = (int) (brokerJson.GetProperty("UptimeMillis").GetUInt64() / 1000);
        var id = $"amq-{agentConfig.Server}-{agentConfig.Port}";
        var dataService = new DataService(id, this.SourceName, agentConfig.Server, "Queue Broker", "ActiveMQ")
        {
            OnlineRespositories = list.Count(),
            Status = "Online",
            Version = BrokerVersion,
            ServiceStartTime = DateTime.Now - new TimeSpan(0, 0, 0, UptimeSec)
        };
        await _ingestApi.IngestResource(new[] { dataService });

        var queueList = new List<QueueResource>();
        foreach (var queue in list)
        {
            var objectName = queue.GetProperty("objectName").GetString();
            var items = objectName.Split(',');
            var properties = new Dictionary<string, string>();
            foreach (var item in items)
            {
                var parts = item.Split('=');
                properties.Add(parts[0], parts[1]);
            }
            var destinationName = properties["destinationName"];
            var destinationType = properties["destinationType"];

            // http://host:port/api/jolokia/read/org.apache.activemq:type=Broker,brokerName=localhost,destinationName=XeoStaging.FileNotify,destinationType=Queue
            uri = new Uri(urlPrefix + $"/api/jolokia/read/{objectName}");
            content = await client.GetStringAsync(uri);
            json = JsonDocument.Parse(content);

            var queueJson = json.RootElement.GetProperty("value");

            var ConsumerCount = queueJson.GetProperty("ConsumerCount").GetInt64();
            var ProducerCount = queueJson.GetProperty("ProducerCount").GetInt64();
            var EnqueueCount = queueJson.GetProperty("EnqueueCount").GetInt64();
            var DequeueCount = queueJson.GetProperty("DequeueCount").GetInt64();
            var QueueSize = queueJson.GetProperty("QueueSize").GetInt64();
            var AverageMessageSize = queueJson.GetProperty("AverageMessageSize").GetInt64();

            var queueResource = new QueueResource(
                $"{agentConfig.Server}.{destinationType}.{destinationName}",
                this.SourceName,
                agentConfig.Server, destinationName)
            {
                ConsumerCount = ConsumerCount,
                ProducerCount = ProducerCount,
                EnqueueCount = EnqueueCount,
                DequeueCount = DequeueCount,
                QueueSize = QueueSize,
                ResourceType = destinationType,
                AverageMessageSize = AverageMessageSize
            };
            queueList.Add(queueResource);
        }
        await _ingestApi.IngestResource(queueList);
    }
}

