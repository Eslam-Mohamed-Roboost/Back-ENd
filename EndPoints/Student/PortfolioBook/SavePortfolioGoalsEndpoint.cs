using API.Application.Features.Student.PortfolioBook.Commands;
using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Filters;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.EndPoints.Student.PortfolioBook;

public class SavePortfolioGoalsEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/Student/PortfolioBook/Goals",
                async (IMediator mediator, [FromBody] SavePortfolioGoalsRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new SavePortfolioGoalsCommand(request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<PortfolioGoalsDto>>();
    }
}
