using API.Application.Features.Student.Activity.DTOs;
using API.Application.Features.Student.Activity.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Student.Activity;

public class GetPointsSummaryEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Student/Points",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetPointsSummaryQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<PointsSummaryDto>>();
    }
}
