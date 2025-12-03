using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.User.Register.Commands;

public record AddUserCommand(
    string Name,
    string UserName,
    string Email,
    string Password,
    string SaltPassword,
    string PhoneNumber,
    ApplicationRole Role) : IRequest<long>;

public class AddUserCommandHandler : RequestHandlerBase<AddUserCommand, long>
{
    private readonly IRepository<Domain.Entities.User> _repository;
    
    public AddUserCommandHandler(RequestHandlerBaseParameters parameters, IRepository<Domain.Entities.User> repository) : base(parameters)
    {
        _repository = repository;
    }

    public override async Task<long> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        long entity = _repository.Add(new Domain.Entities.User
        {
            Name = request.Name,
            UserName = request.UserName,
            Email = request.Email,
            Password = request.Password,
            PhoneNumber = request.PhoneNumber,
            Role = request.Role,
            SaltPassword = request.SaltPassword,
            IsActive = true
        });

        return await Task.FromResult(entity);
    }
}

