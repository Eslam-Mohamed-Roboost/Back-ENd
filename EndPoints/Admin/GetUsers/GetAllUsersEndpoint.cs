using API.Application.Features.Admin.GetUsers.DTOs;
using API.Application.Features.Admin.GetUsers.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.GetUsers;

public class GetAllUsersEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/Users",
                async (IMediator mediator, int page, int pageSize, string? search, int? role, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetAllUsersQuery(page, pageSize, search, role), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<PagingDto<AdminUserDto>>>();
    }
}
