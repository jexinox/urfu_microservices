namespace Gateway.Infrastructure.Mongo.Configuration;

public record MongoAuthenticationOptions
{
    public required string Username { get; init; }
    
    public required string Password { get; init; }
}