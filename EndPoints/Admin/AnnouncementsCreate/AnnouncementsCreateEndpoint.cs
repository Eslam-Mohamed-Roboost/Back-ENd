using API.Application.Features.Admin.Announcements.Commands;
using API.Application.Features.Admin.Announcements.DTOs;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.AnnouncementsCreate;

public class AnnouncementsCreateEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Admin/Announcements",
                async (IMediator mediator, CreateAnnouncementDto request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new CreateAnnouncementCommand(request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<long>>();
    }
}
