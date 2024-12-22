namespace Gateway.Notifications.Domain;

public record NotificationEntity(Guid Id, NotificationStatus Status, Notification Notification);

public enum NotificationStatus
{
    Created,
    Fail,
    Success,
}