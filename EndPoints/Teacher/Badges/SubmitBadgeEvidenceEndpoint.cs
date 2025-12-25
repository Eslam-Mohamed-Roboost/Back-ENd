using API.Application.Features.Teacher.Badges.Commands;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Teacher.Badges;

public class SubmitBadgeEvidenceEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Teacher/Badges/Submit",
                async (IMediator mediator, SubmitBadgeEvidenceCommand command, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(command, cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<long>>();
    }
}

