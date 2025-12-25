using API.Application.Features.Student.Badges.Commands;
using API.Application.Features.Student.Badges.DTOs;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Student.Badges;

public class AwardBadgeEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Student/Badges/Award",
                async (IMediator mediator, AwardBadgeRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new AwardBadgeCommand(request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();
    }
}
