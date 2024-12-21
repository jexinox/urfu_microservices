using Kontur.Results;

namespace Gateway.Notifications.Domain;

public interface INotificationPublisher
{
    bool ShouldPublish(NotificationType type);
    
    Task<Result<NotificationPublishError>> Publish(NotificationEntity notification);
}

public record NotificationPublishError(NotificationPublishErrorType Type, string? Message = null);

public enum NotificationPublishErrorType
{
    InvalidData,
    TransportError,
}