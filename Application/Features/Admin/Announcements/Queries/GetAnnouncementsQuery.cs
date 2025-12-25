using API.Application.Features.Admin.Announcements.DTOs;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AnnouncementsEntity = API.Domain.Entities.System.Announcements;

namespace API.Application.Features.Admin.Announcements.Queries;

public record GetAnnouncementsQuery(int Page = 1, int PageSize = 20) 
    : IRequest<RequestResult<PagingDto<AnnouncementDto>>>;

public class GetAnnouncementsQueryHandler(RequestHandlerBaseParameters parameters, IRepository<AnnouncementsEntity> repository)
    : RequestHandlerBase<GetAnnouncementsQuery, RequestResult<PagingDto<AnnouncementDto>>>(parameters)
{
    public override async Task<RequestResult<PagingDto<AnnouncementDto>>> Handle(GetAnnouncementsQuery request, CancellationToken cancellationToken)
    {
        var query = repository.Get();
        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var announcements = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new AnnouncementDto
            {
                Id = a.ID,
                Title = a.Title,
                Content = a.Content,
                Priority = (int)a.Priority,
                PriorityName = a.Priority.ToString(),
                IsPinned = a.IsPinned,
                ShowAsPopup = a.ShowAsPopup,
                SendEmail = a.SendEmail,
                PublishedAt = a.PublishedAt,
                ViewCount = a.ViewCount,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var result = new PagingDto<AnnouncementDto>(request.PageSize, request.Page, totalCount, totalPages, announcements);

        return RequestResult<PagingDto<AnnouncementDto>>.Success(result);
    }
}
