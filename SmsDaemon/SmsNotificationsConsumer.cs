using Gateway.QueueModels;
using MassTransit;

namespace SmsDaemon;

public class SmsNotificationsConsumer(ILogger<SmsNotificationsConsumer> logger, IBus bus) : IConsumer<SmsNotification>
{
    public async Task Consume(ConsumeContext<SmsNotification> context)
    {
        var message = context.Message;
        logger.LogInformation(
            "Sms notification received, phone number: {phone}, message: {message}",
            message.Number,
            message.Message);
        await bus.Publish(new NotificationStatusChange(message.Id, NewNotificationStatus.Success));
    }
}