using Kontur.Results;

namespace Gateway.Notifications.Domain.Persistence;

public interface INotificationsRepository
{
    Task<Result<NotificationsRepositoryCreateError, NotificationEntity>> Create(Notification notification);
    
    Task<Result<NotificationsRepositoryGetError, NotificationEntity>> Get(Guid notificationId);
}

public enum NotificationsRepositoryGetError
{
    NotFound,
    DatabaseError,
}

public enum NotificationsRepositoryCreateError
{
    DatabaseError,
}