using Gateway.Infrastructure.Mongo.Configuration;
using MongoDB.Driver;

namespace Gateway.Infrastructure.Mongo;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoClient(this IServiceCollection serviceCollection, string mongoOptionsSection = MongoOptions.Section)
    {
        serviceCollection
            .AddOptions<MongoOptions>()
            .BindConfiguration(mongoOptionsSection, options => options.ErrorOnUnknownConfiguration = true);

        serviceCollection
            .AddSingleton<IMongoUrlProvider, MongoUrlProvider>()
            .AddSingleton<MongoClientFactory>()
            .AddSingleton<IMongoClient>(sp => sp.GetRequiredService<MongoClientFactory>().Create());

        return serviceCollection;
    }
}