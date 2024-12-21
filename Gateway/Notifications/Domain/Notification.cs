namespace Gateway.Notifications.Domain;

public record Notification(
    NotificationType Type,
    IReadOnlyDictionary<string, string> Metadata);

[Flags]
public enum NotificationType
{
    Email = 1,
    Sms = 2,
}