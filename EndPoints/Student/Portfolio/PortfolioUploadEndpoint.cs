using API.Application.Features.Student.Portfolio.Commands;
using API.Application.Features.Student.Portfolio.DTOs;
using API.Filters;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.EndPoints.Student.Portfolio;

public class PortfolioUploadEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Student/Portfolio/Upload",
                async (IMediator mediator, [FromForm] PortfolioUploadRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new UploadPortfolioFileCommand(request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .DisableAntiforgery()
            .Produces<EndPointResponse<PortfolioFileDto>>();
    }
}


