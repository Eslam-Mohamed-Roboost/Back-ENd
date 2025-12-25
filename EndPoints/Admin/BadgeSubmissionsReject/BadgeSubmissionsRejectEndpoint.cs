using API.Application.Features.Admin.BadgeSubmissions.DTOs;
using API.Filters;
using API.Shared.Models;

namespace API.EndPoints.Admin.BadgeSubmissionsReject;

public class BadgeSubmissionsRejectEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Admin/BadgeSubmissions/{id}/Reject",
                (long id, ReviewBadgeSubmissionDto request, CancellationToken cancellationToken) =>
                {
                    var result =   RequestResult<dynamic>.Success(new { id, status = "Rejected", reviewedBy = request.ReviewedBy, reviewDate = DateTime.UtcNow, reviewNotes = request.ReviewNotes }, "Announcement deleted successfully");
                    return Response(result);
                     
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
