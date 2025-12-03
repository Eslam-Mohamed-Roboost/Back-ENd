using API.Domain.Entities.Identity;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.User.Login.Commands;

public record AddTokenCommand(long UserID, long JwtID) : IRequest<long>;

public class AddTokenCommandHandler : RequestHandlerBase<AddTokenCommand, long>
{
    private readonly IRepository<Token> _repository;

    public AddTokenCommandHandler(RequestHandlerBaseParameters parameters, IRepository<Token> repository) : base(parameters)
    {
        _repository = repository;
    }

    public override async Task<long> Handle(AddTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenID = _repository.Add(new Token
        {
            UserID = request.UserID,
            JwtID = request.JwtID
        });

        return await Task.FromResult(tokenID);
    }
}

