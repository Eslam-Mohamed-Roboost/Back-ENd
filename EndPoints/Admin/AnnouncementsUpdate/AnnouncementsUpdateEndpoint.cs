using API.Filters;
using API.Shared.Models;

namespace API.EndPoints.Admin.AnnouncementsUpdate;

public class AnnouncementsUpdateEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/Admin/Announcements/{id}",
                (long id, object request, CancellationToken cancellationToken) =>
                {
                    var result =   RequestResult<bool>.Success(true, "Announcement deleted successfully");
                    return Response(result);
                 })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
