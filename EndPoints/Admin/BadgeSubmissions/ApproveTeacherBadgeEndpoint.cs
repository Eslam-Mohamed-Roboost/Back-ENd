using API.Application.Features.Admin.BadgeSubmissions.Commands;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.BadgeSubmissions;

public class ApproveTeacherBadgeEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/Admin/BadgeSubmissions/{submissionId}/Approve",
                async (IMediator mediator, long submissionId, ApproveTeacherBadgeCommand command, CancellationToken cancellationToken) =>
                {
                    // Ensure submissionId from route matches command
                    var updatedCommand = command with { SubmissionId = submissionId };
                    var result = await mediator.Send(updatedCommand, cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();
    }
}

