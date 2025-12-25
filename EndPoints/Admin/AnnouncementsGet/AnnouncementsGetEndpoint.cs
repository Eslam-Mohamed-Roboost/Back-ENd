using API.Application.Features.Admin.Announcements.DTOs;
using API.Application.Features.Admin.Announcements.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.AnnouncementsGet;

public class AnnouncementsGetEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/Announcements",
                async (IMediator mediator, int page, int pageSize, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetAnnouncementsQuery(page, pageSize), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<PagingDto<AnnouncementDto>>>();
    }
}
