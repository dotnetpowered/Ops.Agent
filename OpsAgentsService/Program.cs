using Ops.Agents;
using Ops.Agents.Service;
using Refit;

// TODO: Look at https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks

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
        services.AddTransient<Ops.Agents.Amq.AmqAgent>()
                .AddTransient<Ops.Agents.Aws.AwsEc2Agent>()
                .AddTransient<Ops.Agents.Azure.AzureUpdateAgent>()
                .AddTransient<Ops.Agents.Azure.AzureComputeAgent>()
                .AddTransient<Ops.Agents.Elasticsearch.ElasticsearchAgent>()
                .AddTransient<Ops.Agents.Google.Cloud.GoogleComputeAgent>()
                .AddTransient<Ops.Agents.RedHat.Insights.InsightsAgent>()
                .AddTransient<Ops.Agents.MongoDB.MongoDbAgent>()
                .AddTransient<Ops.Agents.NewRelic.NewRelicAgent>()
                .AddTransient<Ops.Agents.Ntirety.NtiretyAgent>()
                .AddTransient<Ops.Agents.Octopus.OctopusAgent>()
                .AddTransient<Ops.Agents.Rundeck.RundeckAgent>()
                .AddTransient<Ops.Agents.SqlServer.SqlServerAgent>()
                .AddTransient<Ops.Agents.vSphere.vSphereAgent>()
                .AddTransient<Ops.Agents.Zabbix.ZabbixAgent>();

        services.AddLogging(lf => lf.AddConsole());

        services.AddHostedService<Worker>();

        services
            .AddRefitClient<IOpsIngestApi>()
            .ConfigureHttpClient(c => {
                c.BaseAddress = new Uri(Configuration["service:ingestUrl"]);
                c.DefaultRequestHeaders.Add("x-functions-key",
                    Configuration["service:ingestApiCode"]);
            });
    })
    .Build();

await host.RunAsync();

