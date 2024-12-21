using Kontur.Results;
using MassTransit;

namespace Gateway.Notifications.Domain;

public class BusNotificationHandler<TNotification>(
    NotificationType notificationType,
    INotificationMapper<TNotification> mapper,
    IBus bus,
    ILogger<BusNotificationHandler<TNotification>> logger) : INotificationHandler 
    where TNotification : class
{
    public bool ShouldHandle(NotificationType type) => type == notificationType;

    public async Task<Result<NotificationHandleError>> Handle(Notification notification)
    {
        var notificationMapResult = mapper.Map(notification);

        if (notificationMapResult.TryGetFault(out var fault, out var outNotification))
        {
            return new NotificationHandleError(NotificationHandleErrorType.InvalidData, fault.Message);
        }

        try
        {
            await bus.Publish(outNotification);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send notification of type {notificationType}", notificationType);
            return new NotificationHandleError(NotificationHandleErrorType.TransportError);
        }
        
        return Result.Succeed();
    }
}