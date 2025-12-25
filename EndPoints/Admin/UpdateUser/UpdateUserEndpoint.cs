using API.Application.Features.Admin.UpdateUser.Commands;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.UpdateUser;

public class UpdateUserEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/Admin/Users/{userId}",
                async (IMediator mediator, long userId, UpdateUserRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new UpdateUserCommand(
                        userId,
                        request.Name,
                        request.Email,
                        request.Role,
                        request.IsActive,
                        request.PhoneNumber,
                        request.ClassID), cancellationToken);

                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();
    }
}

public class UpdateUserRequest
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public Domain.Enums.ApplicationRole? Role { get; set; }
    public bool? IsActive { get; set; }
    public string? PhoneNumber { get; set; }
    public long? ClassID { get; set; }
}

