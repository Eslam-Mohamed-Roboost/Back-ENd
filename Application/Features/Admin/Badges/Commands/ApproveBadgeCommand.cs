using API.Domain.Entities.Gamification;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BadgeEntity = API.Domain.Entities.Users.Badges;
using Status = API.Domain.Entities.Gamification.Status;

namespace API.Application.Features.Admin.Badges.Commands;

public record ApproveBadgeCommand(long BadgeSubmissionId) : IRequest<RequestResult<bool>>;

public class ApproveBadgeCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<StudentBadges> studentBadgeRepository,
    IRepository<BadgeEntity> badgeRepository,
    IRepository<Domain.Entities.User> userRepository)
    : RequestHandlerBase<ApproveBadgeCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(
        ApproveBadgeCommand request,
        CancellationToken cancellationToken)
    {
        // Note: Assuming StudentBadges has a Status field or we're checking if it exists
        var studentBadge = await studentBadgeRepository.Get(sb => sb.ID == request.BadgeSubmissionId)
            .FirstOrDefaultAsync(cancellationToken);

        if (studentBadge == null)
        {
            return RequestResult<bool>.Failure(ErrorCode.NotFound, "Badge submission not found");
        }

        // Mark as approved
        studentBadge.Status = Status.Approved;
        studentBadge.UpdatedAt = DateTime.UtcNow;
        studentBadgeRepository.Update(studentBadge);
        await studentBadgeRepository.SaveChangesAsync();

        // TODO: Create notification for student (Phase 7)

        return RequestResult<bool>.Success(true, "Badge approved successfully");
    }
}

