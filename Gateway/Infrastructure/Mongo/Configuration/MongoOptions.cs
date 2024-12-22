namespace Gateway.Infrastructure.Mongo.Configuration;

public record MongoOptions
{
    public const string Section = "Mongo";
    
    public required MongoAuthenticationOptions Authentication { get; init; }
    
    public required IReadOnlyCollection<string> Hosts { get; init; }
    
    public required string Database { get; init; }
}