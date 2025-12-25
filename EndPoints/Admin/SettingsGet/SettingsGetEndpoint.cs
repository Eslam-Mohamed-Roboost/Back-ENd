using API.Application.Features.Admin.Settings.DTOs;
using API.Application.Features.Admin.Settings.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.SettingsGet;

public class SettingsGetEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/Settings",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetSystemSettingsQuery(), cancellationToken);
                    return Response(result);
                 })
            .WithTags("Admin")
            .Produces<EndPointResponse<SystemSettingsDto>>()
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
