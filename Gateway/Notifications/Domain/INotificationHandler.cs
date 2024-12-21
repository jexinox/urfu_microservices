using Kontur.Results;

namespace Gateway.Notifications.Domain;

public interface INotificationHandler
{
    bool ShouldHandle(NotificationType type);
    
    Task<Result<NotificationHandleError>> Handle(Notification notification);
}

public record NotificationHandleError(NotificationHandleErrorType Type, string? Message = null);

public enum NotificationHandleErrorType
{
    InvalidData,
    TransportError,
}