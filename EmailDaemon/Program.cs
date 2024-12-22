using System.Reflection;
using EmailDaemon;
using EmailDaemon.Infrastructure;
using MassTransit;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureLogging();
builder.Services.AddMassTransit(massTransit =>
{
    massTransit.SetKebabCaseEndpointNameFormatter();
    massTransit.AddConsumers(Assembly.GetExecutingAssembly());
    massTransit.UsingRabbitMq((context, configurator) =>
    {
        configurator.ConfigureEndpoints(context);
    });
});
builder.Services
    .AddOptions<RabbitMqTransportOptions>()
    .BindConfiguration("RabbitMq");
builder.Services
    .AddOpenTelemetry()
    .WithMetrics(metricsBuilder =>
    {
        metricsBuilder
            .AddPrometheusExporter()
            .AddAspNetCoreInstrumentation()
            .AddProcessInstrumentation();
    });

var app = builder.Build();

app.MapPrometheusScrapingEndpoint();
app.Run();