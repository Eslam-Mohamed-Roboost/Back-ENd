using API.Filters;
using API.Shared.Models;

namespace API.EndPoints.Admin.AnnouncementsDelete;

public class AnnouncementsDeleteEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/Admin/Announcements/{id}",
                (long id, CancellationToken cancellationToken) =>
                {
                    
                    var result =   RequestResult<bool>.Success(true, "Announcement deleted successfully");
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
