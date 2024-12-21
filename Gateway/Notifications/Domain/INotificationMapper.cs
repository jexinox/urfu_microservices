using Kontur.Results;

namespace Gateway.Notifications.Domain;

public interface INotificationMapper<T>
{
    Result<NotificationMapError, T> Map(NotificationEntity entity);
}

public record NotificationMapError(string Message);
