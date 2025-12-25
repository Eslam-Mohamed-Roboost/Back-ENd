using API.Application.Features.Student.PortfolioBook.Commands;
using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Filters;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.EndPoints.Student.PortfolioBook;

public class SavePortfolioProfileEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/Student/PortfolioBook/Profile",
                async (IMediator mediator, [FromBody] SavePortfolioProfileRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new SavePortfolioProfileCommand(request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<PortfolioProfileDto>>();
    }
}
