using API.Application.Features.User.Login.DTOs;
using API.Application.Features.User.Login.Queries;
using API.Domain.Enums;
using API.EndPoints.User.Login;
using API.Shared.Helpers;
using API.Shared.Models;
using IdGen;
using MediatR;

namespace API.Application.Features.User.Login.Commands;

public record UserLoginOrchestrator(string UserName, string Password) : IRequest<RequestResult<UserLoginResponse>>;

public class UserLoginOrchestratorHandler : RequestHandlerBase<UserLoginOrchestrator, RequestResult<UserLoginResponse>>
{
    public UserLoginOrchestratorHandler(RequestHandlerBaseParameters parameters) : base(parameters)
    {
    }

    public override async Task<RequestResult<UserLoginResponse>> Handle(UserLoginOrchestrator request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
        {
            return RequestResult<UserLoginResponse>.Failure(ErrorCode.EmptyUserNameOrPassword);
        }

        var encryptedUserName = SecurityHelper.Encrypt(request.UserName.ToLower().Trim());
        
        UserLoginDTO? user = await _mediator.Send(new GetUserByUserNameQuery(encryptedUserName), cancellationToken);

        var hashedPassword = user != null ? SecurityHelper.GetHashedPassword(request.Password, user.SaltPassword) : null;

        if (user == null || user.Password != hashedPassword)
        {
            return RequestResult<UserLoginResponse>.Failure(ErrorCode.InvalidUserNameOrPassword);
        }

        long jwtID = new IdGenerator(0).CreateId();

        user.Authorization = await _mediator.Send(new GenerateAccessTokenCommand(jwtID, user.ID, user.Role), cancellationToken);

        await _mediator.Send(new AddTokenCommand(user.ID, jwtID), cancellationToken);

        return RequestResult<UserLoginResponse>.Success(new UserLoginResponse
        {
            ID = user.ID,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            Authorization = user.Authorization,
            PhoneNumber = user.PhoneNumber
        }, "User logged in successfully.");
    }
}

