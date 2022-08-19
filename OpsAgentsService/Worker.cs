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
                    case "amq":
                        agent = _provider.GetService<Amq.AmqAgent>();
                        break;
                    case "azure":
                        agent = _provider.GetService<Azure.AzureAgent>();
                        break;
                    case "octopus":
                        agent = _provider.GetService<Octopus.OctopusAgent>();
                        break;
                    case "redhat":
                        agent = _provider.GetService<RedHat.Insights.InsightsAgent>();
                        break;
                    case "rundeck":
                        agent = _provider.GetService<Rundeck.RundeckAgent>();
                        break;
                    case "vSphere":
                        agent = _provider.GetService<vSphere.vSphereAgent>();
                        break;
                    case "zabbix":
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

