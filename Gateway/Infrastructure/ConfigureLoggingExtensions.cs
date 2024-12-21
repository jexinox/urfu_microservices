using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Serilog;

namespace Gateway.Infrastructure;

public static class ConfigureLoggingExtensions
{
    public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder applicationBuilder)
    {
        applicationBuilder.Host.UseSerilog((context, loggerConfiguration) =>
        {
            var config = context.Configuration.GetSection("Serilog").Get<SerilogConfiguration>();

            if (config is null)
            {
                throw new ApplicationException("Serilog configuration is missing");
            }
            
            loggerConfiguration
                .WriteTo.Console()
                .WriteTo.Elasticsearch(
                    [new(config.Elasticsearch.Url)],
                    configureTransport: transportConfiguration => 
                        transportConfiguration
                            .Authentication(
                                new BasicAuthentication(config.Elasticsearch.Username, config.Elasticsearch.Password)));
        });
        
        return applicationBuilder;
    }
    
    private record SerilogConfiguration
    {
        public ElasticsearchConfiguration Elasticsearch { get; set; } = default!;
    }

    private record ElasticsearchConfiguration
    {
        public string Url { get; set; } = default!;
        
        public string Username { get; set; } = default!;
        
        public string Password { get; set; } = default!;
    }
}