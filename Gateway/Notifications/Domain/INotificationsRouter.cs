using Kontur.Results;

namespace Gateway.Notifications.Domain;

public interface INotificationsRouter
{
    Task<Result<RouteError>> Route(Notification notification);
}

public record RouteError(RouteErrorType Type, string? Message);

public enum RouteErrorType
{
    InvalidData,
    TransportError,
}