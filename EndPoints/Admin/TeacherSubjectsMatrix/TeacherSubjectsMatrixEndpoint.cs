using API.Application.Features.Admin.TeacherSubjects.DTOs;
using API.Application.Features.Admin.TeacherSubjects.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.TeacherSubjectsMatrix;

public class TeacherSubjectsMatrixEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/TeacherSubjects/Matrix",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetTeacherSubjectsMatrixQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .Produces<EndPointResponse<List<TeacherSubjectMatrixDto>>>()
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
