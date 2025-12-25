using API.Application.Features.Admin.UpdateUser.Commands;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.UpdateUser;

public class UpdateUserStatusRequest
{
    public long UserId { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateUserStatusEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/Admin/Users/Status",
                async (IMediator mediator, UpdateUserStatusRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new UpdateUserStatusCommand(request.UserId, request.IsActive), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();
    }
}
