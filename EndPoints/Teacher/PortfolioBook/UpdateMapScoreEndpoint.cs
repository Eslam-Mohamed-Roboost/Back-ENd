using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Application.Features.Teacher.PortfolioBook.Commands;
using API.Filters;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.EndPoints.Teacher.PortfolioBook;

public class UpdateMapScoreEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/Teacher/PortfolioBook/MapScore",
                async (IMediator mediator, [FromBody] UpdateMapScoreRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new UpdateMapScoreCommand(request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<PortfolioMapScoreDto>>();
    }
}
