using API.Application.Features.Admin.Announcements.DTOs;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using AnnouncementsEntity = API.Domain.Entities.System.Announcements;

namespace API.Application.Features.Admin.Announcements.Commands;

public record CreateAnnouncementCommand(CreateAnnouncementDto Dto) : IRequest<RequestResult<long>>;

public class CreateAnnouncementCommandHandler(RequestHandlerBaseParameters parameters, IRepository<AnnouncementsEntity> repository)
    : RequestHandlerBase<CreateAnnouncementCommand, RequestResult<long>>(parameters)
{
    public override async Task<RequestResult<long>> Handle(CreateAnnouncementCommand request, CancellationToken cancellationToken)
    {
        var announcement = new AnnouncementsEntity
        {
            Title = request.Dto.Title,
            Content = request.Dto.Content,
            Priority = (AnnouncementPriority)request.Dto.Priority,
            TargetAudience = request.Dto.TargetAudience,
            IsPinned = request.Dto.IsPinned,
            ShowAsPopup = request.Dto.ShowAsPopup,
            SendEmail = request.Dto.SendEmail,
            PublishedAt = request.Dto.PublishNow ? DateTime.UtcNow : null,
            ViewCount = 0,
            CreatedAt = DateTime.UtcNow
        };

        var id = repository.Add(announcement);
        await repository.SaveChangesAsync();

        return RequestResult<long>.Success(id, "Announcement created successfully");
    }
}
