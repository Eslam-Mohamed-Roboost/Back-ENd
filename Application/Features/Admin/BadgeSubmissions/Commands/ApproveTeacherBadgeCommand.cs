using API.Application.Services;
using API.Domain.Entities.Teacher;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.BadgeSubmissions.Commands;

public record ApproveTeacherBadgeCommand(
    long SubmissionId,
    bool Approved,
    string? ReviewNotes = null,
    decimal? CpdHoursOverride = null) : IRequest<RequestResult<bool>>;

public class ApproveTeacherBadgeCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<TeacherBadgeSubmissions> submissionsRepository,
    IRepository<API.Domain.Entities.Gamification.Badges> badgesRepository,
    INotificationService notificationService,
    IHoursTrackingService hoursTrackingService)
    : RequestHandlerBase<ApproveTeacherBadgeCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(
        ApproveTeacherBadgeCommand request,
        CancellationToken cancellationToken)
    {
        var submission = await submissionsRepository
            .Get(x => x.ID == request.SubmissionId)
            .FirstOrDefaultAsync(cancellationToken);

        if (submission == null)
        {
            return RequestResult<bool>.Failure(ErrorCode.NotFound, "Badge submission not found");
        }

        if (submission.Status != SubmissionStatus.Pending)
        {
            return RequestResult<bool>.Failure(
                ErrorCode.ValidationError,
                $"This submission has already been {submission.Status.ToString().ToLower()}");
        }

        // Update submission status
        submission.Status = request.Approved ? SubmissionStatus.Approved : SubmissionStatus.Rejected;
        submission.ReviewedBy = _userState.UserID;
        submission.ReviewedAt = DateTime.UtcNow;
        submission.ReviewNotes = request.ReviewNotes;

        if (request.Approved && request.CpdHoursOverride.HasValue)
        {
            submission.CpdHoursAwarded = request.CpdHoursOverride.Value;
        }

        submissionsRepository.Update(submission);
        await submissionsRepository.SaveChangesAsync();

        // If approved, record CPD hours
        if (request.Approved && submission.CpdHoursAwarded.HasValue && submission.CpdHoursAwarded.Value > 0)
        {
            await hoursTrackingService.RecordCpdHoursAsync(
                submission.TeacherId,
                submission.BadgeId,
                submission.CpdHoursAwarded.Value,
                cancellationToken);
        }

        // Get badge details for notification
        var badge = await badgesRepository
            .Get(x => x.ID == submission.BadgeId)
            .FirstOrDefaultAsync(cancellationToken);

        if (badge != null)
        {
            // Notify teacher
            await notificationService.SendBadgeApprovalNotificationAsync(
                submission.TeacherId,
                badge.Name,
                request.Approved,
                request.ReviewNotes,
                badge.ID,
                cancellationToken);
        }

        var statusText = request.Approved ? "approved" : "rejected";
        return RequestResult<bool>.Success(true, $"Badge submission {statusText} successfully");
    }
}

