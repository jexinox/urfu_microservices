using Gateway.Infrastructure;
using Gateway.Notifications.Api;
using MassTransit;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogging();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMassTransit(massTransit =>
{
    massTransit.SetKebabCaseEndpointNameFormatter();
    massTransit.UsingRabbitMq();
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
builder.Services.AddNotifications();

builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateOnBuild = true;
    options.ValidateScopes = true;
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(); 

app.UsePathBase("/api/v1");
app
    .MapNotifications()
    .MapPrometheusScrapingEndpoint();

app.Run();