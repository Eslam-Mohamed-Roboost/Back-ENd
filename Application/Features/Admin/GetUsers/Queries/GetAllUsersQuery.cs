using API.Application.Features.Admin.GetUsers.DTOs;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.GetUsers.Queries;

public record GetAllUsersQuery(int Page = 1, int PageSize = 20, string? Search = null, int? Role = null) 
    : IRequest<RequestResult<PagingDto<AdminUserDto>>>;

public class GetAllUsersQueryHandler(RequestHandlerBaseParameters parameters, IRepository<Domain.Entities.User> repository)
    : RequestHandlerBase<GetAllUsersQuery, RequestResult<PagingDto<AdminUserDto>>>(parameters)
{
    public override async Task<RequestResult<PagingDto<AdminUserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var query = repository.Get();

        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(u => u.Name.Contains(request.Search) || 
                                     u.Email.Contains(request.Search) ||
                                     u.UserName.Contains(request.Search));
        }

        if (request.Role.HasValue)
        {
            query = query.Where(u => (int)u.Role == request.Role.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new AdminUserDto
            {
                Id = u.ID,
                Name = u.Name,
                UserName = u.UserName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Role = (int)u.Role,
                RoleName = u.Role.ToString(),
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                BadgesCount = u.Badges != null ? u.Badges.Count : 0
            })
            .ToListAsync(cancellationToken);

        var result = new PagingDto<AdminUserDto>(request.PageSize, request.Page, totalCount, totalPages, users);

        return RequestResult<PagingDto<AdminUserDto>>.Success(result);
    }
}
