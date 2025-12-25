using API.Application.Features.Student.Notifications.Commands;
using API.Filters;
using API.Helpers.Attributes;
using API.Shared.Models;
using MediatR;
using System.Text.Json.Serialization;

namespace API.EndPoints.Student.Notifications;

public class MarkNotificationAsReadEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/Student/Notifications/{notificationId}/Read",
                async (IMediator mediator, [AsParameters] MarkNotificationRouteParams routeParams, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new MarkNotificationAsReadCommand(routeParams.NotificationId), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();
    }
}

public class MarkNotificationRouteParams
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long NotificationId { get; set; }
}
