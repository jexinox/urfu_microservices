using Gateway.Infrastructure.Mongo.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Gateway.Infrastructure.Mongo;

public class MongoUrlProvider(IOptions<MongoOptions> mongoOptions) : IMongoUrlProvider
{
    public MongoUrl Get()
    {
        var options = mongoOptions.Value;
        
        var mongoUrlBuilder = new MongoUrlBuilder
        {
            Username = options.Authentication.Username,
            Password = options.Authentication.Password,
            DatabaseName = options.Database,
            Servers = options.Hosts.Select(MongoServerAddress.Parse).ToList(), 
        };
        
        return mongoUrlBuilder.ToMongoUrl();
    }
}