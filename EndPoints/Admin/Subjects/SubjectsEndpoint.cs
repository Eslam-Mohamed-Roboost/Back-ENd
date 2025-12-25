using API.Application.Features.Student.Portfolio.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.Subjects;

public class SubjectsEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        // GET /Admin/Subjects - Get all active subjects for dropdowns
        app.MapGet("/Admin/Subjects",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetAllActiveSubjectsQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<API.Application.Features.Student.Portfolio.DTOs.SimpleSubjectDto>>>();
    }
}

