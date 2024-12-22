using Gateway.QueueModels;
using MassTransit;

namespace EmailDaemon;

public class EmailNotificationsErrorConsumer(ILogger<EmailNotificationsConsumer> logger) : IConsumer<Fault<EmailNotification>>
{
    public Task Consume(ConsumeContext<Fault<EmailNotification>> context)
    {
        var errors = string.Join(Environment.NewLine, context.Message.Exceptions.Select(e => e.Message));
        logger.LogError("Email notification error received, {errors}", errors);
        return Task.CompletedTask;
    }
}