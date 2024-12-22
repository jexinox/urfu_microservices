using System.Text;
using Gateway.QueueModels;
using MassTransit;

namespace EmailDaemon;

public class EmailNotificationsErrorConsumer(ILogger<EmailNotificationsConsumer> logger) : IConsumer<Fault<EmailNotification>>
{
    public Task Consume(ConsumeContext<Fault<EmailNotification>> context)
    {
        var errors = new StringBuilder();
        foreach (var exception in context.Message.Exceptions)
        {
            errors.AppendLine(exception.Message);
        }
        
        logger.LogError("Email notification error received, {error}", errors.ToString());
        return Task.CompletedTask;
    }
}