using API.Application.Features.Student.Portfolio.Commands;
using API.Application.Features.Student.Portfolio.DTOs;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Student.Portfolio;

public class ReflectionSaveEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Student/Portfolio/Reflection",
                async (IMediator mediator, SaveReflectionRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new SaveReflectionCommand(request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<ReflectionDto>>();
    }
}
