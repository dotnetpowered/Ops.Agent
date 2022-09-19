using Ops.Agents;
using Ops.Agents.Service;
using Refit;

// Initialize Configuration so compiler won't complain about null reference, even though it is initialized below
var configBuilder = new ConfigurationBuilder();
IConfiguration Configuration = configBuilder.Build();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((HostBuilderContext builderContext, IConfigurationBuilder configurationBuilder) =>
    {
        Configuration = configurationBuilder.Build();
    })
    .ConfigureServices(services =>
    {
        services.AddTransient<Ops.Agents.Rundeck.RundeckAgent>()
                .AddTransient<Ops.Agents.Zabbix.ZabbixAgent>()
                .AddTransient<Ops.Agents.vSphere.vSphereAgent>()
                .AddTransient<Ops.Agents.Octopus.OctopusAgent>()
                .AddTransient<Ops.Agents.Azure.AzureUpdateAgent>()
                .AddTransient<Ops.Agents.Azure.AzureComputeAgent>()
                .AddTransient<Ops.Agents.Amq.AmqAgent>()
                .AddTransient<Ops.Agents.Aws.AwsEc2Agent>()
                .AddTransient<Ops.Agents.NewRelic.NewRelicAgent>()
                .AddTransient<Ops.Agents.RedHat.Insights.InsightsAgent>();

        services.AddLogging(lf => lf.AddConsole());

        services.AddHostedService<Worker>();

        services
            .AddRefitClient<IOpsIngestApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration["Ingest:Url"]));
    })
    .Build();

await host.RunAsync();

