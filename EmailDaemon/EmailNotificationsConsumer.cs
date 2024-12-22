using Gateway.QueueModels;
using MassTransit;

namespace EmailDaemon;

public class EmailNotificationsConsumer(ILogger<EmailNotificationsConsumer> logger) : IConsumer<EmailNotification>
{
    public Task Consume(ConsumeContext<EmailNotification> context)
    {
        logger.LogInformation("Email notification received");
        return Task.CompletedTask;
    }
}