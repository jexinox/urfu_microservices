using Gateway.QueueModels;
using Kontur.Results;

namespace Gateway.Notifications.Domain.Sms;

public class SmsMapper : INotificationMapper<SmsNotification>
{
    private const string NumberFieldName = "Number";
    private const string MessageFieldName = "Message";
    
    public Result<NotificationMapError, SmsNotification> Map(NotificationEntity entity)
    {
        var meta = entity.Notification.Metadata;

        if (!meta.TryGetValue(NumberFieldName, out var number))
        {
            return new NotificationMapError("Number is required");
        }

        if (!meta.TryGetValue(MessageFieldName, out var message))
        {
            return new NotificationMapError("Message is required");
        }

        return new SmsNotification(entity.Id, number, message);
    }
}