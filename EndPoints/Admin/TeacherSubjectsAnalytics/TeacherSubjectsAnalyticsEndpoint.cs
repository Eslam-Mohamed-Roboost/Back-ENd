using API.Application.Features.Admin.TeacherSubjects.DTOs;
using API.Application.Features.Admin.TeacherSubjects.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.TeacherSubjectsAnalytics;

public class TeacherSubjectsAnalyticsEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/TeacherSubjects/Analytics",
                async (IMediator mediator, string? subject, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetSubjectAnalyticsQuery(subject), cancellationToken);
                    return Response(result);
                 })
            .WithTags("Admin")
            .Produces<EndPointResponse<SubjectAnalyticsDto>>()
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
