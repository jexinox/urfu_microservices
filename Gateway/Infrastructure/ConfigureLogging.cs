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
            var config = new SerilogConfiguration();
            context.Configuration.GetSection("Serilog").Bind(config);
            
            loggerConfiguration
                .WriteTo.Console()
                .WriteTo.Elasticsearch(
                    [new(config.Elasticsearch.Url)],
                    configureTransport: options => 
                        options
                            .Authentication(
                                new BasicAuthentication(config.Elasticsearch.User, config.Elasticsearch.Password)));
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
        
        public string User { get; set; } = default!;
        
        public string Password { get; set; } = default!;
    }
}