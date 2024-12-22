namespace Gateway.Notifications.Api;

public record CreateNotificationRequest(NotificationType Type, IReadOnlyDictionary<string, string> Metadata);