using MongoDB.Driver;

namespace Gateway.Infrastructure.Mongo;

public class MongoClientFactory(IMongoUrlProvider mongoUrlProvider)
{
    public IMongoClient Create()
    {
        return new MongoClient(mongoUrlProvider.Get());
    }
}