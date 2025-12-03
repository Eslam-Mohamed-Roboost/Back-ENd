using API.Application.Services;
using API.Domain.Enums;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.User.Login.Commands;

public record GenerateAccessTokenCommand(long JwtID, long UserID, ApplicationRole Role) : IRequest<string>;

public class GenerateAccessTokenCommandHandler : RequestHandlerBase<GenerateAccessTokenCommand, string>
{
    private readonly JwtService _jwtService;

    public GenerateAccessTokenCommandHandler(RequestHandlerBaseParameters parameters, JwtService jwtService) : base(parameters)
    {
        _jwtService = jwtService;
    }

    public override async Task<string> Handle(GenerateAccessTokenCommand request, CancellationToken cancellationToken)
    {
        var token = _jwtService.GenerateAccessToken(request.JwtID, request.UserID.ToString(), request.Role.ToString());
        return await Task.FromResult(token ?? string.Empty);
    }
}

