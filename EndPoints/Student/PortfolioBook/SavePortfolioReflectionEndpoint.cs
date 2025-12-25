using API.Application.Features.Student.PortfolioBook.Commands;
using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Filters;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.EndPoints.Student.PortfolioBook;

public class SavePortfolioReflectionEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Student/PortfolioBook/Reflection",
                async (IMediator mediator, [FromBody] SavePortfolioReflectionRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new SavePortfolioReflectionCommand(request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<PortfolioReflectionDto>>();
    }
}
