using Kontur.Results;
using MassTransit;

namespace Gateway.Notifications.Domain;

public class BusNotificationPublisher<TNotification>(
    NotificationType notificationType,
    INotificationMapper<TNotification> mapper,
    IBus bus,
    ILogger<BusNotificationPublisher<TNotification>> logger) : INotificationPublisher 
    where TNotification : class
{
    public bool ShouldPublish(NotificationType type) => type == notificationType;

    public async Task<Result<NotificationPublishError>> Publish(NotificationEntity notification)
    {
        var notificationMapResult = mapper.Map(notification);

        if (notificationMapResult.TryGetFault(out var fault, out var outNotification))
        {
            return new NotificationPublishError(NotificationPublishErrorType.InvalidData, fault.Message);
        }

        try
        {
            await bus.Publish(outNotification);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send notification of type {notificationType}", notificationType);
            return new NotificationPublishError(NotificationPublishErrorType.TransportError);
        }
        
        logger.LogInformation("Successfully sent notification of type {notificationType}", notificationType);
        return Result.Succeed();
    }
}