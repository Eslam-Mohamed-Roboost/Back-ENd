using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Application.Features.Teacher.PortfolioBook.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.EndPoints.Teacher.PortfolioBook;

public class GetStudentPortfolioBookEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Teacher/PortfolioBook/Student/{studentId}/{subjectId}",
                async (IMediator mediator, [FromRoute] string studentId, [FromRoute] string subjectId, CancellationToken cancellationToken) =>
                {
                    if (!long.TryParse(studentId, out var studentIdLong) || !long.TryParse(subjectId, out var subjectIdLong))
                        return Results.BadRequest(new { Message = "Invalid StudentId or SubjectId format." });

                    var result = await mediator.Send(new GetStudentPortfolioBookQuery(studentIdLong, subjectIdLong), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .AddEndpointFilter<TeacherRoleFilter>()
            .Produces<EndPointResponse<PortfolioBookDto>>();
    }
}
