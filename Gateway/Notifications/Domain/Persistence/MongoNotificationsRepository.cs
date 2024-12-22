using Gateway.QueueModels;
using Kontur.Results;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Gateway.Notifications.Domain.Persistence;

public class MongoNotificationsRepository(
    IMongoDatabase mongoDatabase,
    ILogger<MongoNotificationsRepository> logger,
    IOptions<MongoNotificationsRepositoryConfiguration> config) : INotificationsRepository
{
    private readonly Lazy<IMongoCollection<MongoNotification>> notificationsCollection = new(() => GetNotificationsCollection(mongoDatabase, config));
    
    public async Task<Result<NotificationsRepositoryCreateError, NotificationEntity>> Create(Notification notification)
    {
        try
        {
            var collection = notificationsCollection.Value;
            var mapped = Map(notification);
            await collection.InsertOneAsync(mapped);

            return new NotificationEntity(mapped.Id, Map(mapped.Status), notification);
        }
        catch (MongoException ex)
        {
            logger.LogError(ex, "Failed to create notification, notification: {notification}", notification);
            return NotificationsRepositoryCreateError.DatabaseError;
        }
    }

    public async Task<Result<NotificationsRepositoryGetError, NotificationEntity>> Get(Guid notificationId)
    {
        try
        {
            var collection = notificationsCollection.Value;
            var mongoNotification = await collection
                .Find(notification => notification.Id == notificationId)
                .FirstOrDefaultAsync();
            
            if (mongoNotification is null)
            {
                return NotificationsRepositoryGetError.NotFound;
            }

            return Map(mongoNotification);
        }
        catch (MongoException ex)
        {
            logger.LogError(ex, "Error while getting notification from MongoDB, id: {notificationId}", notificationId);
            return NotificationsRepositoryGetError.DatabaseError;
        }
    }

    public async Task<Result<NotificationsRepositoryChangeStatusError>> ChangeStatus(Guid notificationId, NotificationStatus status)
    {
        try
        {
            var collection = notificationsCollection.Value;
            var filter = Builders<MongoNotification>.Filter.Eq(notification => notification.Id, notificationId);
            var update = Builders<MongoNotification>.Update.Set(notification => notification.Status, Map(status));
            var updateResult = await collection.UpdateOneAsync(filter, update);

            if (updateResult.ModifiedCount == 0)
            {
                return NotificationsRepositoryChangeStatusError.NotFound;
            }

            return Result.Succeed();
        }
        catch (MongoException ex)
        {
            logger.LogError(ex, "Error while changing notification state, id {notificationId}", notificationId);
            return NotificationsRepositoryChangeStatusError.DatabaseError;
        }
    }

    private static MongoNotification Map(Notification notification)
    {
        var type = notification.Type switch
        {
            NotificationType.Email => MongoNotificationType.Email,
            NotificationType.Sms => MongoNotificationType.Sms,
            _ => throw new ArgumentOutOfRangeException(nameof(notification), notification, null),
        };

        return new(Guid.NewGuid(), type, MongoNotificationStatus.Created, notification.Metadata);
    }

    private static NotificationEntity Map(MongoNotification mongoNotification)
    {
        var type = mongoNotification.Type switch
        {
            MongoNotificationType.Email => NotificationType.Email,
            MongoNotificationType.Sms => NotificationType.Sms,
            _ => throw new ArgumentOutOfRangeException(nameof(mongoNotification), mongoNotification, null),
        };
        
        return new(mongoNotification.Id, Map(mongoNotification.Status), new(type, mongoNotification.Metadata));
    }

    private static NotificationStatus Map(MongoNotificationStatus mongoNotificationStatus) =>
        mongoNotificationStatus switch
        {
            MongoNotificationStatus.Created => NotificationStatus.Created,
            MongoNotificationStatus.Fail => NotificationStatus.Fail,
            MongoNotificationStatus.Success => NotificationStatus.Success,
            _ => throw new ArgumentOutOfRangeException(nameof(mongoNotificationStatus), mongoNotificationStatus, null),
        };
    
    private static MongoNotificationStatus Map(NotificationStatus mongoNotificationStatus) =>
        mongoNotificationStatus switch
        {
            NotificationStatus.Created => MongoNotificationStatus.Created,
            NotificationStatus.Fail => MongoNotificationStatus.Fail,
            NotificationStatus.Success => MongoNotificationStatus.Success,
            _ => throw new ArgumentOutOfRangeException(nameof(mongoNotificationStatus), mongoNotificationStatus, null),
        };

    private static IMongoCollection<MongoNotification> GetNotificationsCollection(
        IMongoDatabase mongoDatabase,
        IOptions<MongoNotificationsRepositoryConfiguration> config)
    {
        return mongoDatabase.GetCollection<MongoNotification>(config.Value.Collection);
    }

    private record MongoNotification(
        [property: BsonGuidRepresentation(GuidRepresentation.Standard)] Guid Id, 
        MongoNotificationType Type,
        MongoNotificationStatus Status,
        IReadOnlyDictionary<string, string> Metadata);

    private enum MongoNotificationType
    {
        Sms,
        Email,
    }

    private enum MongoNotificationStatus
    {
        Created,
        Fail,
        Success,
    }
}