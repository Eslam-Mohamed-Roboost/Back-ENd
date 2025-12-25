using API.Application.Features.Student.Activity.Commands;
using API.Application.Features.Student.Activity.DTOs;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Student.Activity;

public class AwardPointsEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Student/Points/Award",
                async (IMediator mediator, AwardPointsRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new AwardPointsCommand(request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();
    }
}
