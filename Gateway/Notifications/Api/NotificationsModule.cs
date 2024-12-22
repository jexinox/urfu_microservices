using Gateway.Infrastructure.Mongo;
using Gateway.Infrastructure.Mongo.Configuration;
using Gateway.QueueModels;
using Gateway.Notifications.Domain;
using Gateway.Notifications.Domain.Email;
using Gateway.Notifications.Domain.Persistence;
using Gateway.Notifications.Domain.Sms;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using DomainNotificationType = Gateway.Notifications.Domain.NotificationType;
using DomainNotificationStatus = Gateway.Notifications.Domain.NotificationStatus;

namespace Gateway.Notifications.Api;

public static class NotificationsModule
{
    public static IServiceCollection AddNotifications(this IServiceCollection services)
    {
        return services
            .AddSingleton<INotificationsRouter, NotificationsRouter>()
            .AddSingleton<INotificationPublisher, BusNotificationPublisher<SmsNotification>>(sp =>
                new(
                    DomainNotificationType.Sms,
                    sp.GetRequiredService<INotificationMapper<SmsNotification>>(),
                    sp.GetRequiredService<IBus>(),
                    sp.GetRequiredService<ILogger<BusNotificationPublisher<SmsNotification>>>()))
            .AddSingleton<INotificationPublisher, BusNotificationPublisher<EmailNotification>>(sp =>
                new(
                    DomainNotificationType.Email,
                    sp.GetRequiredService<INotificationMapper<EmailNotification>>(),
                    sp.GetRequiredService<IBus>(),
                    sp.GetRequiredService<ILogger<BusNotificationPublisher<EmailNotification>>>()))
            .AddSingleton<INotificationMapper<SmsNotification>, SmsMapper>()
            .AddSingleton<INotificationMapper<EmailNotification>, EmailMapper>()
            .AddMongoClient()
            .AddSingleton<INotificationsRepository, MongoNotificationsRepository>()
            .AddOptions<MongoNotificationsRepositoryConfiguration>()
            .BindConfiguration(MongoNotificationsRepositoryConfiguration.Section).Services
            .AddSingleton<IMongoDatabase>(sp => 
                sp
                    .GetRequiredService<IMongoClient>()
                    .GetDatabase(sp.GetRequiredService<IOptions<MongoOptions>>().Value.Database));
    }

    public static IEndpointRouteBuilder MapNotifications(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapPost("/notifications", HandleCreateNotificationRequest)
            .WithOpenApi();

        endpoints
            .MapGet("/notifications/{id:guid}", HandleGetNotificationRequest)
            .WithOpenApi();
        
        return endpoints;
    }

    private static async Task<Results<
            Ok<CreateNotificationResponse>,
            BadRequest<ApiError>,
            UnprocessableEntity<ApiError>,
            ProblemHttpResult>> 
        HandleCreateNotificationRequest(
            [FromBody] CreateNotificationRequest request, 
            [FromServices] INotificationsRouter router)
    {
        var domainModel = Map(request);
        var routingResult = await router.Route(domainModel);

        if (routingResult.TryGetValue(out var id, out var fault))
        {
            return TypedResults.Ok(new CreateNotificationResponse(id));
        }
        
        var apiError = new ApiError(fault.Type.ToString(), fault.Message);
        if (fault.Type is RouteErrorType.NoPossibleRoute)
        {
            return TypedResults.UnprocessableEntity(apiError);
        }

        if (fault.Type is RouteErrorType.RepositoryError)
        {
            return TypedResults.Problem();
        }
            
        return TypedResults.BadRequest(apiError);

    }

    private static async Task<Results<Ok<GetNotificationResponse>, NotFound<ApiError>, ProblemHttpResult>> HandleGetNotificationRequest(
        [FromRoute] Guid id,
        [FromServices] INotificationsRepository repository)
    {
        var getNotificationResult = await repository.Get(id);
        if (getNotificationResult.TryGetValue(out var value, out var fault))
        {
            return TypedResults.Ok(Map(value));
        }

        var apiError = new ApiError(fault.ToString(), null);

        if (fault is NotificationsRepositoryGetError.NotFound)
        {
            return TypedResults.NotFound(apiError);
        }

        return TypedResults.Problem();
    }

    private static Notification Map(CreateNotificationRequest request)
    {
        var type = request.Type switch
        {
            NotificationType.Email => DomainNotificationType.Email,
            NotificationType.Sms => DomainNotificationType.Sms,
            _ => throw new ArgumentOutOfRangeException(nameof(request), request, null)
        };

        return new(type, request.Metadata);
    }

    private static GetNotificationResponse Map(NotificationEntity entity)
    {
        var type = entity.Notification.Type switch
        {
            DomainNotificationType.Email => NotificationType.Email,
            DomainNotificationType.Sms => NotificationType.Sms,
            _ => throw new ArgumentOutOfRangeException(nameof(entity), entity, null)
        };

        var status = entity.Status switch
        {
            DomainNotificationStatus.Created => NotificationStatus.Created,
            DomainNotificationStatus.Fail => NotificationStatus.Fail,
            DomainNotificationStatus.Success => NotificationStatus.Success,
            _ => throw new ArgumentOutOfRangeException(nameof(entity), entity, null)
        };
        
        return new(entity.Id, type, status, entity.Notification.Metadata);
    }
}