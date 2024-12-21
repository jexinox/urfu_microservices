using Gateway.Notifications.Domain.Persistence;
using Kontur.Results;

namespace Gateway.Notifications.Domain;

public class NotificationsRouter(
    IEnumerable<INotificationPublisher> publishers,
    INotificationsRepository repository) : INotificationsRouter
{
    public async Task<Result<RouteError>> Route(Notification notification)
    {
        foreach (var publisher in publishers)
        {
            if (!publisher.ShouldPublish(notification.Type))
            {
                continue;
            }

            var createRecordResult = await repository.Create(notification);
            if (createRecordResult.TryGetFault(out var createRecordFault, out var entity))
            {
                return new RouteError(RouteErrorType.RepositoryError, createRecordFault.ToString());
            }
            
            var publishResult = await publisher.Publish(entity);
            if (publishResult.TryGetFault(out var publishFault))
            {
                return new RouteError(MapHandlerErrorType(publishFault.Type), publishFault.Message);
            }
        }

        return Result.Succeed();
    }

    private RouteErrorType MapHandlerErrorType(NotificationPublishErrorType publishErrorType) =>
        publishErrorType switch
        {
            NotificationPublishErrorType.InvalidData => RouteErrorType.InvalidData,
            NotificationPublishErrorType.TransportError => RouteErrorType.TransportError,
            _ => throw new ArgumentOutOfRangeException(nameof(publishErrorType), publishErrorType, null)
        };
}