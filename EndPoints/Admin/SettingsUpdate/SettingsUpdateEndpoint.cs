using API.Filters;
using API.Shared.Models;

namespace API.EndPoints.Admin.SettingsUpdate;

public class SettingsUpdateEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/Admin/Settings",
                (object request, CancellationToken cancellationToken) =>
                {
                     
                    var result1 =   RequestResult<bool>.Success(true);
                    return Response(result1);
                 })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
