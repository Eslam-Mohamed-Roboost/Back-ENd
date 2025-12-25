using API.Application.Features.Student.Dashboard.DTOs;
using API.Application.Features.Student.Notifications.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Student.Notifications;

public class GetNotificationsEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Student/Notifications",
                async (IMediator mediator, bool? unreadOnly, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetNotificationsQuery(unreadOnly), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<NotificationDto>>>();
    }
}
