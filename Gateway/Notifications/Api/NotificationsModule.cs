using Gateway.Notifications.Domain;
using Gateway.Notifications.Domain.Email;
using Gateway.Notifications.Domain.Sms;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using DomainNotificationType = Gateway.Notifications.Domain.NotificationType;

namespace Gateway.Notifications.Api;

public static class NotificationsModule
{
    public static IServiceCollection AddNotifications(this IServiceCollection services)
    {
        return services
            .AddSingleton<INotificationsRouter, NotificationsRouter>()
            .AddSingleton<INotificationHandler, BusNotificationHandler<SmsNotification>>(sp =>
                new(
                    DomainNotificationType.Sms,
                    sp.GetRequiredService<INotificationMapper<SmsNotification>>(),
                    sp.GetRequiredService<IBus>(),
                    sp.GetRequiredService<ILogger<BusNotificationHandler<SmsNotification>>>()))
            .AddSingleton<INotificationHandler, BusNotificationHandler<EmailNotification>>(sp =>
                new(
                    DomainNotificationType.Email,
                    sp.GetRequiredService<INotificationMapper<EmailNotification>>(),
                    sp.GetRequiredService<IBus>(),
                    sp.GetRequiredService<ILogger<BusNotificationHandler<EmailNotification>>>()))
            .AddSingleton<INotificationMapper<SmsNotification>, SmsMapper>()
            .AddSingleton<INotificationMapper<EmailNotification>, EmailMapper>();
    }

    public static IEndpointRouteBuilder MapNotifications(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapPost("/notifications", HandleCreateNotificationRequest)
            .WithOpenApi();

        return endpoints;
    }

    private static async Task<Results<Ok, BadRequest<ApiError>>> HandleCreateNotificationRequest(
        [FromBody] CreateNotificationRequest request,
        [FromServices] INotificationsRouter router)
    {
        var domainModel = Map(request);
        var routingResult = await router.Route(domainModel);

        if (routingResult.TryGetFault(out var fault))
        {
            var apiError = new ApiError(fault.Type.ToString(), fault.Message);
            return TypedResults.BadRequest(apiError);
        }
        
        return TypedResults.Ok();
    }

    private static Notification Map(CreateNotificationRequest request)
    {
        var type = request.Type switch
        {
            NotificationType.Email => DomainNotificationType.Email,
            NotificationType.Sms => DomainNotificationType.Sms,
            _ => throw new ArgumentOutOfRangeException(nameof(request), request, "Invalid notification type")
        };

        return new(type, request.Metadata);
    }
}