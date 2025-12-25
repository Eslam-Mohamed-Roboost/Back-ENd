using API.Application.Features.Student.Badges.DTOs;
using API.Domain.Entities.Gamification;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.Badges.Commands;

public record AwardBadgeCommand(AwardBadgeRequest Request) : IRequest<RequestResult<bool>>;

public class AwardBadgeCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<StudentBadges> studentBadgesRepository,
    IRepository<Domain.Entities.Gamification.Badges> badgesRepository)
    : RequestHandlerBase<AwardBadgeCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(AwardBadgeCommand request, CancellationToken cancellationToken)
    {
        // Check if badge exists
        var badge = await badgesRepository.Get(x => x.ID == request.Request.BadgeId && x.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        if (badge == null)
            return RequestResult<bool>.Failure(ErrorCode.NotFound);

        // Check if student already has this badge
        var existingBadge = await studentBadgesRepository.Get(x => 
            x.StudentId == request.Request.StudentId && 
            x.BadgeId == request.Request.BadgeId)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingBadge != null)
            return RequestResult<bool>.Failure(ErrorCode.BadRequest); // Already awarded

        // Award the badge
        var studentBadge = new StudentBadges
        {
            StudentId = request.Request.StudentId,
            BadgeId = request.Request.BadgeId,
            EarnedDate = DateTime.UtcNow,
            AutoAwarded = true,
            Status = Status.Approved
        };

         studentBadgesRepository.Add(studentBadge);

        // TODO: Create notification for student
        // TODO: Update student level if threshold reached

        return RequestResult<bool>.Success(true);
    }
}
