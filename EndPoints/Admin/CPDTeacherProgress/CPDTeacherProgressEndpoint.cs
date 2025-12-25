using API.Application.Features.Admin.CPD.DTOs;
using API.Application.Features.Admin.CPD.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.CPDTeacherProgress;

public class CPDTeacherProgressEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/CPD/TeacherProgress",
                async (IMediator mediator, string? sortBy, string? sortDirection, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetCPDTeacherProgressQuery(sortBy, sortDirection), cancellationToken);
                    return Response(result);
                 })
            .WithTags("Admin")
            .Produces<EndPointResponse<List<TeacherCPDProgressDto>>>()
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
