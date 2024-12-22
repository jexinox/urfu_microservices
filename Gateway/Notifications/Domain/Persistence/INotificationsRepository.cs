using Kontur.Results;

namespace Gateway.Notifications.Domain.Persistence;

public interface INotificationsRepository
{
    Task<Result<NotificationsRepositoryCreateError, NotificationEntity>> Create(Notification notification);
    
    Task<Result<NotificationsRepositoryGetError, NotificationEntity>> Get(Guid notificationId);
    
    Task<Result<NotificationsRepositoryChangeStatusError>> ChangeStatus(Guid notificationId, NotificationStatus state);
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

public enum NotificationsRepositoryChangeStatusError
{
    NotFound,
    DatabaseError,
}