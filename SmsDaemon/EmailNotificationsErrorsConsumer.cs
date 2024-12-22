using Gateway.QueueModels;
using MassTransit;

namespace SmsDaemon;

public class SmsNotificationsErrorConsumer(ILogger<SmsNotificationsConsumer> logger) : IConsumer<Fault<SmsNotification>>
{
    public Task Consume(ConsumeContext<Fault<SmsNotification>> context)
    {
        var errors = string.Join(Environment.NewLine, context.Message.Exceptions.Select(e => e.Message));
        logger.LogError("Sms notification error received, {errors}", errors);
        return Task.CompletedTask;
    }
}