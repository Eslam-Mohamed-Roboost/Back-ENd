using API.Application.Features.User.GetUsers.DTOs;
  using API.Application.Features.User.GetUsers.Queriess;
  using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.User.GetUsers;

public class GetUsersEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/User/GetUsers",
                async (IMediator _mediator,[AsParameters] GetUserRequest request, CancellationToken cancellationToken) =>
                {
                     var result = await _mediator.Send(new GetUsersQuery(request.email,request.role,request.IsActve,request.page,request.pageSize), cancellationToken);
                    return Response(result);
                })
            .WithTags("User")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<PagingDto<UserListDto>>>();
    }
}