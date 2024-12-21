using Kontur.Results;

namespace Gateway.Notifications.Domain;

public interface INotificationMapper<T>
{
    Result<NotificationMapError, T> Map(Notification notification);
}

public record NotificationMapError(string Message);
