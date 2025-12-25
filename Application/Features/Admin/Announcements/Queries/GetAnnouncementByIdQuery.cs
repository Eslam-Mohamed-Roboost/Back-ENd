using API.Application.Features.Admin.Announcements.DTOs;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AnnouncementEntity = API.Domain.Entities.System.Announcements;

namespace API.Application.Features.Admin.Announcements.Queries;

public record GetAnnouncementByIdQuery(long Id) : IRequest<RequestResult<AnnouncementDto>>;

public class GetAnnouncementByIdQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<AnnouncementEntity> repository)
    : RequestHandlerBase<GetAnnouncementByIdQuery, RequestResult<AnnouncementDto>>(parameters)
{
    public override async Task<RequestResult<AnnouncementDto>> Handle(GetAnnouncementByIdQuery request, CancellationToken cancellationToken)
    {
        var announcement = await repository.Get(a => a.ID == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (announcement == null)
        {
            return RequestResult<AnnouncementDto>.Failure(ErrorCode.NotFound, "Announcement not found");
        }

        return RequestResult<AnnouncementDto>.Success(announcement);
    }
}

