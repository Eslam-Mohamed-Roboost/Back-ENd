using API.Application.Features.Admin.Announcements.DTOs;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AnnouncementsEntity = API.Domain.Entities.System.Announcements;
using AdminAnnouncementDto = API.Application.Features.Admin.Announcements.DTOs.AnnouncementDto;

namespace API.Application.Features.Teacher.Lounge.Queries;

public record GetTeacherAnnouncementsQuery(int Limit = 20) : IRequest<RequestResult<List<AdminAnnouncementDto>>>;

public class GetTeacherAnnouncementsQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<AnnouncementsEntity> repository)
    : RequestHandlerBase<GetTeacherAnnouncementsQuery, RequestResult<List<AnnouncementDto>>>(parameters)
{
    public override async Task<RequestResult<List<AnnouncementDto>>> Handle(GetTeacherAnnouncementsQuery request, CancellationToken cancellationToken)
    {
        // Get announcements that are published and target teachers
        var announcements = await repository.Get()
            .Where(a => a.PublishedAt.HasValue && a.PublishedAt <= DateTime.UtcNow)
            .OrderByDescending(a => a.IsPinned)
            .ThenByDescending(a => a.PublishedAt)
            .Take(request.Limit)
            .Select(a => new AdminAnnouncementDto
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

        return RequestResult<List<AnnouncementDto>>.Success(announcements);
    }
}

