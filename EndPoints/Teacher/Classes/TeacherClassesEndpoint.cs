using API.Application.Features.Teacher.Classes.DTOs;
using API.Application.Features.Teacher.Classes.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Teacher.Classes;

public class TeacherClassesEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Teacher/Classes",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetMyClassesQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<TeacherClassDto>>>();

        app.MapGet("/Teacher/Classes/{classId}/Students",
                async (IMediator mediator, long classId, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetClassStudentsQuery(classId), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<ClassStudentDto>>>();
    }
}

