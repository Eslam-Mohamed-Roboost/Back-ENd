using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.User.Register.Queries;

public record GetUserNameQuery(string UserName) : IRequest<bool>;

public class GetUserNameQueryHandler : RequestHandlerBase<GetUserNameQuery, bool>
{
    private readonly IRepository<Domain.Entities.User> _repository;

    public GetUserNameQueryHandler(RequestHandlerBaseParameters parameters, IRepository<Domain.Entities.User> repository) : base(parameters)
    {
        _repository = repository;
    }

    public override async Task<bool> Handle(GetUserNameQuery request, CancellationToken cancellationToken)
    {
        return await _repository.Get(x => x.UserName == request.UserName)
            .AnyAsync(cancellationToken);
    }
}

