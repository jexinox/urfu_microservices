namespace Gateway.Notifications.Domain.Persistence;

public record MongoNotificationsRepositoryConfiguration
{
    public const string Section = "MongoNotificationsRepository";
    
    public required string Collection { get; init; }
};