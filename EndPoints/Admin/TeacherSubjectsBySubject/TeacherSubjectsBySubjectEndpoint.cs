using API.Application.Features.Admin.TeacherSubjects.DTOs;
using API.Application.Features.Admin.TeacherSubjects.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.TeacherSubjectsBySubject;

public class TeacherSubjectsBySubjectEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/TeacherSubjects/BySubject",
                async (IMediator mediator, string? subject, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetTeacherSubjectsBySubjectQuery(subject), cancellationToken);
                    return Response(result);
                 })
            .WithTags("Admin")
            .Produces<EndPointResponse<List<TeacherSubjectMatrixDto>>>()
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
