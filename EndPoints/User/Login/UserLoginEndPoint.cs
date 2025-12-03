using API.Application.Features.User.Login.Commands;
using API.Middlewares;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.User.Login;

public class UserLoginEndPoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/user/login",
            async (IMediator mediator, UserLoginRequest request, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new UserLoginOrchestrator(
                    request.UserName,
                    request.Password), cancellationToken);

                return Response(result);
            })
        .WithTags("User")
        .AddEndpointFilter<ValidationFilter<UserLoginRequest>>()
        .Produces<EndPointResponse<UserLoginResponse>>();
    }
}

