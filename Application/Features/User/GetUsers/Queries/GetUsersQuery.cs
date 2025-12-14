using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using API.Application.Features.User.GetUsers.DTOs;
using API.Domain.Enums;
using API.Helpers;
using LinqKit;

using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.User.GetUsers.Queriess;

public record GetUsersQuery(string? Email, ApplicationRole? Role,bool? IsActive  ,int Page = 1, int Size = 10) : IRequest<RequestResult<PagingDto<UserListDto>>>;

public class GetUsersQueryHandler(
    RequestHandlerBaseParameters parameters, 
    IRepository<Domain.Entities.User> userRepository,
    IRepository<Domain.Entities.Users.Badges> badgesRepository,
    IRepository<Domain.Entities.Identity.Token> tokenRepository)
    : RequestHandlerBase<GetUsersQuery, RequestResult<PagingDto<UserListDto>>>(parameters)
{
    public override async Task<RequestResult<PagingDto<UserListDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        // Get badge counts per user
        // var badgeCounts = badgesRepository.Get()
        //     .GroupBy(b => b.UserId)
        //     .Select(g => new { UserId = g.Key, Count = g.Count() });

        // Get last login per user (make LastLogin nullable for LEFT JOIN)
        // var lastLogins = tokenRepository.Get()
        //     .GroupBy(t => t.UserID)
        //     .Select(g => new { UserId = g.Key, LastLogin = (DateTime?)g.Max(x => x.CreatedAt) });

        // Main query with left joins

        var pridicate = CreatePredicate(request);
        var query = userRepository.Get(pridicate)
            .Select(x => new UserListDto
            {
                Id = x.ID,
                Name = x.Name,
                Email = x.Email,
                Role = x.Role.GetDescription(),
                Status = x.IsActive ? "Active" : "Inactive",
                LastLogin = x.LastLogin,
                BadgeCount = x.Badges.Count()

            });

        var result = await query.ToPagingDtoAsync(request.Page, request.Size, cancellationToken);
 
        return RequestResult<PagingDto<UserListDto>>.Success(result);
    }
    
    
    private ExpressionStarter<Domain.Entities.User> CreatePredicate(GetUsersQuery query)
    {
        var predicate = PredicateBuilder.New<Domain.Entities.User>(true);


        if (!string.IsNullOrEmpty(query.Email))
        {
            predicate = predicate.And(ch => ch.Email.ToLower().Contains(query.Email.ToLower()));
            
        }

        if (query.Role is not null)
        {
            predicate = predicate.And(ch => ch.Role ==  query.Role);
            
        }
        if (query.IsActive is not null)
        {
            predicate = predicate.And(ch => ch.IsActive ==  query.IsActive);

        }

        return predicate;
    }
}

 