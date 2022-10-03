using Ops.Agents;

namespace Ops.Agents.Service;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly AgentConfig[] _config;
    private readonly IServiceProvider _provider;
    private readonly int _pollingFrequency;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, IServiceProvider provider)
    {
        _logger = logger;
        _config = configuration.GetSection("agents").Get<AgentConfig[]>();
        _provider = provider;
        _pollingFrequency = int.Parse(configuration["service:pollingFrequency"]);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            foreach (var agentConfig in _config)
            {
                IOpsAgent? agent = null;
                var agentFound = true;
                _logger.LogInformation($"Running agent: {agentConfig.Agent}");
                switch (agentConfig.Agent)
                {
                    case "ActiveMQ":
                        agent = _provider.GetService<Amq.AmqAgent>();
                        break;
                    case "Aws.EC2":
                        agent = _provider.GetService<Aws.AwsEc2Agent>();
                        break;
                    case "Azure.Compute":
                        agent = _provider.GetService<Azure.AzureComputeAgent>();
                        break;
                    case "Azure.UpdateService":
                        agent = _provider.GetService<Azure.AzureUpdateAgent>();
                        break;
                    case "Google.Compute":
                        agent = _provider.GetService<Google.Cloud.GoogleComputeAgent>();
                        break;
                    case "Octopus.Deploy":
                        agent = _provider.GetService<Octopus.OctopusAgent>();
                        break;
                    case "NewRelic":
                        agent = _provider.GetService<NewRelic.NewRelicAgent>();
                        break;
                    case "PagerDuty.Rundeck":
                        agent = _provider.GetService<Rundeck.RundeckAgent>();
                        break;
                    case "RedHat.Insights":
                        agent = _provider.GetService<RedHat.Insights.InsightsAgent>();
                        break;
                    case "SqlServer":
                        agent = _provider.GetService<SqlServer.SqlServerAgent>();
                        break;
                    case "VMWare.vSphere":
                        agent = _provider.GetService<vSphere.vSphereAgent>();
                        break;
                    case "Zabbix":
                        agent = _provider.GetService<Zabbix.ZabbixAgent>();
                        break;
                    default:
                        _logger.LogWarning($"Unknown agent name: {agentConfig.Agent}. Skippping collection.");
                        agentFound = false;
                        continue;
                }
                if (agent != null)
                    await agent.CollectAsync(agentConfig);
                else if (agentFound)
                    throw new InvalidOperationException($"Missing DI for agent: {agentConfig.Agent}");
            }

            await Task.Delay(_pollingFrequency * 1000, stoppingToken);
        }
    }
}

