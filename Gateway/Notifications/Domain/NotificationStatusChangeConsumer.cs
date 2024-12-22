using Gateway.Notifications.Domain.Persistence;
using Gateway.QueueModels;
using MassTransit;

namespace Gateway.Notifications.Domain;

public class NotificationStatusChangeConsumer(
    ILogger<NotificationStatusChangeConsumer> logger,
    INotificationsRepository repository) : IConsumer<NotificationStatusChange>
{
    public async Task Consume(ConsumeContext<NotificationStatusChange> context)
    {
        var notification = context.Message;
        var changeResult = await repository.ChangeStatus(notification.Id, Map(notification.NewStatus));
        if (changeResult.TryGetFault(out var fault))
        {
            logger.LogError("Error while changing notification state, {error}", fault);
        }
    }

    private static NotificationStatus Map(NewNotificationStatus notificationStatus) =>
        notificationStatus switch
        {
            NewNotificationStatus.Success => NotificationStatus.Success,
            NewNotificationStatus.Fail => NotificationStatus.Fail,
            _ => throw new ArgumentOutOfRangeException(nameof(notificationStatus), notificationStatus, null)
        };
}