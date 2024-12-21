using Kontur.Results;

namespace Gateway.Notifications.Domain;

public class NotificationsRouter(IEnumerable<INotificationHandler> handlers) : INotificationsRouter
{
    public async Task<Result<RouteError>> Route(Notification notification)
    {
        foreach (var handler in handlers)
        {
            if (!handler.ShouldHandle(notification.Type))
            {
                continue;
            }
            
            var handleResult = await handler.Handle(notification);

            if (handleResult.TryGetFault(out var fault))
            {
                return new RouteError(MapHandlerErrorType(fault.Type), fault.Message);
            }
        }

        return Result.Succeed();
    }

    private RouteErrorType MapHandlerErrorType(NotificationHandleErrorType handleErrorType) =>
        handleErrorType switch
        {
            NotificationHandleErrorType.InvalidData => RouteErrorType.InvalidData,
            NotificationHandleErrorType.TransportError => RouteErrorType.TransportError,
            _ => throw new ArgumentOutOfRangeException(nameof(handleErrorType), handleErrorType, null)
        };
}