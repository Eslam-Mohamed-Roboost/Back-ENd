using API.Application.Features.Student.PortfolioBook.Commands;
using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Filters;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.EndPoints.Student.PortfolioBook;

public class SavePortfolioJourneyEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Student/PortfolioBook/Journey",
                async (IMediator mediator, [FromBody] SavePortfolioJourneyRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new SavePortfolioJourneyCommand(request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<PortfolioJourneyEntryDto>>();
    }
}
