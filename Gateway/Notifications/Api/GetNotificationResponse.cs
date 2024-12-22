namespace Gateway.Notifications.Api;

public record GetNotificationResponse(
    Guid Id,
    NotificationType Type,
    NotificationStatus Status,
    IReadOnlyDictionary<string, string> Metadata);

public enum NotificationStatus
{
    Created,
    Fail,
    Success,
}
