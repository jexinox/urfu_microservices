using MongoDB.Driver;

namespace Gateway.Infrastructure.Mongo;

public interface IMongoUrlProvider
{
    MongoUrl Get();
}