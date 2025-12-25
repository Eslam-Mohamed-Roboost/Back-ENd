using API.Application.Features.Teacher.Cpd.DTOs;
using API.Application.Features.Teacher.Dashboard.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Teacher.Dashboard;

public class TeacherDashboardEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Teacher/Dashboard",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetTeacherDashboardQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<TeacherDashboardDto>>();
    }
}


