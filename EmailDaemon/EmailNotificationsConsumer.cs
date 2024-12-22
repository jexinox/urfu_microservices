using Gateway.QueueModels;
using MassTransit;

namespace EmailDaemon;

public class EmailNotificationsConsumer(ILogger<EmailNotificationsConsumer> logger, IBus bus) : IConsumer<EmailNotification>
{
    public async Task Consume(ConsumeContext<EmailNotification> context)
    {
        var message = context.Message;
        logger.LogInformation(
            "Email notification received, address: {address}, subject: {subject}, body: {body}",
            message.Address,
            message.Subject,
            message.Body);
        await bus.Publish(new NotificationStatusChange(message.Id, NewNotificationStatus.Success));
    }
}