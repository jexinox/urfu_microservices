namespace Gateway.QueueModels;

public record NotificationStatusChange(Guid Id, NewNotificationStatus NewStatus);

public enum NewNotificationStatus
{
    Fail,
    Success,
}