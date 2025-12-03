using API.Application.Features.User.Login.DTOs;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API.Application.Features.User.Login.Queries;

public record GetUserByUserNameQuery(string UserName) : IRequest<UserLoginDTO>;

public class GetUserByUserNameQueryHandler : RequestHandlerBase<GetUserByUserNameQuery, UserLoginDTO>
{
    private readonly IRepository<Domain.Entities.User> _repository;
    
    public GetUserByUserNameQueryHandler(RequestHandlerBaseParameters parameters, IRepository<Domain.Entities.User> repository) : base(parameters)
    {
        _repository = repository;
    }

    public override async Task<UserLoginDTO> Handle(GetUserByUserNameQuery request, CancellationToken cancellationToken)
    {
        var data = await _repository.Get(user => user.UserName == request.UserName && user.IsActive)
            .Select(MapToResponse())
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
            
        return data;
    }

    private static Expression<Func<Domain.Entities.User, UserLoginDTO>> MapToResponse()
    {
        return res => new UserLoginDTO
        {
            ID = res.ID,
            UserName = res.UserName,
            Password = res.Password,
            Role = res.Role,
            Name = res.Name,
            Email = res.Email,
            SaltPassword = res.SaltPassword,
            PhoneNumber = res.PhoneNumber
        };
    }
}

