using API.Application.Features.User.Export.DTOs;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using API.Helpers;

namespace API.Application.Features.User.Export.Queries;

public record ExportUserDataQuery() : IRequest<RequestResult<List<UserExportItemDto>>>;

public class ExportUserDataQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.User> userRepository)
    : RequestHandlerBase<ExportUserDataQuery, RequestResult<List<UserExportItemDto>>>(parameters)
{
    public override async Task<RequestResult<List<UserExportItemDto>>> Handle(ExportUserDataQuery request, CancellationToken cancellationToken)
    {
        var query = userRepository.Get()
            .Select(x => new UserExportItemDto
            {
                Id = x.ID,
                Name = x.Name,
                Email = x.Email,
                Role = x.Role.GetDescription(),
                Status = x.IsActive ? "Active" : "Inactive",
                LastLogin = x.LastLogin,
                BadgeCount = x.Badges.Count()
            });

        var data = await query.ToListAsync(cancellationToken);
        
        return RequestResult<List<UserExportItemDto>>.Success(data);
    }
}
