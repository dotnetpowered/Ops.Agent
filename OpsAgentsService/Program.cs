using Ops.Agents;
using Ops.Agents.Service;
using Refit;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var url = "";
        services.AddTransient<Ops.Agents.Rundeck.RundeckAgent>()
                .AddTransient<Ops.Agents.Zabbix.ZabbixAgent>()
                .AddTransient<Ops.Agents.vSphere.vSphereAgent>()
                .AddTransient<Ops.Agents.Octopus.OctopusAgent>()
                .AddTransient<Ops.Agents.Azure.AzureAgent>()
                .AddTransient<Ops.Agents.Amq.AmqAgent>()
                .AddTransient<Ops.Agents.NewRelic.NewRelicAgent>()
                .AddTransient<Ops.Agents.RedHat.Insights.InsightsAgent>();

        services.AddLogging(lf => lf.AddConsole());

        services.AddHostedService<Worker>();

        services
            .AddRefitClient<IOpsIngestApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(url));
    })
    .Build();

await host.RunAsync();

