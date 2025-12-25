using API.Application.Features.Admin.Missions.DTOs;
using API.Application.Features.Teacher.Missions.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Teacher.Missions;

public class TeacherMissionProgressEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        // GET /Teacher/Missions/ClassProgress - Get mission progress for teacher's classes
        app.MapGet("/Teacher/Missions/ClassProgress",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetClassMissionProgressQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<ClassMissionProgressDto>>>();
    }
}

