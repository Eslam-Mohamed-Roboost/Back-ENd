using API.Application.Features.User.Register.Commands;
using API.Middlewares;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.EndPoints.User.Register;

public class RegisterUserEndPoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/user/register",
            async (IMediator mediator,[FromBody]RegisterUserRequest request, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new RegisterUserOrchestrator(
                    request.Name,
                    request.UserName,
                    request.Email,
                    request.Password,
                    request.PhoneNumber,
                    request.RoleID), cancellationToken);

                return Response(result);
            })
        .WithTags("User")
        .AddEndpointFilter<ValidationFilter<RegisterUserRequest>>()
        .Produces<EndPointResponse<bool>>();
    }
}

