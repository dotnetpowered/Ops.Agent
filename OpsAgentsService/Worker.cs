using Ops.Agents;

namespace Ops.Agents.Service;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly AgentConfig[] _config;
    private readonly IServiceProvider _provider;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, IServiceProvider provider)
    {
        _logger = logger;
        _config = configuration.GetSection("agents").Get<AgentConfig[]>();
        _provider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            foreach (var agentConfig in _config)
            {
                IOpsAgent? agent;
                switch (agentConfig.Agent)
                {
                    case "ActiveMQ":
                        agent = _provider.GetService<Amq.AmqAgent>();
                        break;
                    case "Azure.UpdateService":
                        agent = _provider.GetService<Azure.AzureUpdateAgent>();
                        break;
                    case "Azure.Compute":
                        agent = _provider.GetService<Azure.AzureComputeAgent>();
                        break;
                    case "OctopusDeploy":
                        agent = _provider.GetService<Octopus.OctopusAgent>();
                        break;
                    case "RedHat.Insights":
                        agent = _provider.GetService<RedHat.Insights.InsightsAgent>();
                        break;
                    case "PagerDuty.Rundeck":
                        agent = _provider.GetService<Rundeck.RundeckAgent>();
                        break;
                    case "VMWare.vSphere":
                        agent = _provider.GetService<vSphere.vSphereAgent>();
                        break;
                    case "Zabbix":
                        agent = _provider.GetService<Zabbix.ZabbixAgent>();
                        break;
                    default:
                        _logger.LogWarning($"Unknown agent name: {agentConfig.Agent}. Skippping collection.");
                        continue;
                }
                await agent.CollectAsync(agentConfig);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}

