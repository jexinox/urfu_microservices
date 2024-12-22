using System.Net.Mail;
using Gateway.QueueModels;
using Kontur.Results;

namespace Gateway.Notifications.Domain.Email;

public class EmailMapper : INotificationMapper<EmailNotification>
{
    private const string AddressFieldName = "Address";
    private const string SubjectFieldName = "Subject";
    private const string BodyFieldName = "Body";
    
    public Result<NotificationMapError, EmailNotification> Map(NotificationEntity entity)
    {
        var meta = entity.Notification.Metadata;

        if (!meta.TryGetValue(AddressFieldName, out var address) ||
            !MailAddress.TryCreate(address, out _))
        {
            return new NotificationMapError("Invalid email address");
        }

        if (!meta.TryGetValue(SubjectFieldName, out var subject))
        {
            return new NotificationMapError("Subject is required");
        }

        if (!meta.TryGetValue(BodyFieldName, out var body))
        {
            return new NotificationMapError("Body is required");
        }

        return new EmailNotification(entity.Id, address, subject, body);
    }
}