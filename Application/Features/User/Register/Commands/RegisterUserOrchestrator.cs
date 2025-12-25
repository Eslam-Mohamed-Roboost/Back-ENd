using API.Application.Features.User.Register.Queries;
using API.Domain.Enums;
using API.Shared.Helpers;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.User.Register.Commands;

public record RegisterUserOrchestrator(
    string Name, 
    string UserName,
    string Email, 
    string Password,
    string PhoneNumber,
    ApplicationRole Role,
    long? ClassID = null) : IRequest<RequestResult<bool>>;

public class RegisterUserOrchestratorHandler : RequestHandlerBase<RegisterUserOrchestrator, RequestResult<bool>>
{
    public RegisterUserOrchestratorHandler(RequestHandlerBaseParameters parameters) : base(parameters)
    {
    }

    public override async Task<RequestResult<bool>> Handle(RegisterUserOrchestrator request, CancellationToken cancellationToken)
    {
        var encryptedUserName = SecurityHelper.Encrypt(request.UserName.ToLower().Trim());
        
        var existingUser = await _mediator.Send(new GetUserNameQuery(encryptedUserName), cancellationToken);
        
        if (existingUser)
        {
            return RequestResult<bool>.Failure(ErrorCode.UserNameAlreadyExists);
        }

        var saltPassword = SecurityHelper.GenerateSalt();

        var userID = await _mediator.Send(new AddUserCommand(
            request.Name,
            encryptedUserName,
            request.Email,
            SecurityHelper.GetHashedPassword(request.Password, saltPassword),
            saltPassword,
            request.PhoneNumber,
            request.Role,
            request.ClassID), cancellationToken).ConfigureAwait(false);

        return RequestResult<bool>.Success(true, "User registered successfully.");
    }
}

