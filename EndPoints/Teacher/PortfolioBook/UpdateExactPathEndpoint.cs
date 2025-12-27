using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Application.Features.Teacher.PortfolioBook.Commands;
using API.Filters;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.EndPoints.Teacher.PortfolioBook;

public class UpdateExactPathEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/Teacher/PortfolioBook/ExactPath",
                async (IMediator mediator, [FromBody] UpdateExactPathRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new UpdateExactPathCommand(request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .AddEndpointFilter<TeacherRoleFilter>()
            .Produces<EndPointResponse<ExactPathProgressDto>>();
    }
}
