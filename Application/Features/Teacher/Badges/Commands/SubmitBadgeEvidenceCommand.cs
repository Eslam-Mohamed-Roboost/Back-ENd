using API.Application.Services;
using API.Domain.Entities.Teacher;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserEntity = API.Domain.Entities.User;

namespace API.Application.Features.Teacher.Badges.Commands;

public record SubmitBadgeEvidenceCommand(
    long BadgeId,
    string EvidenceLink,
    string? Notes = null) : IRequest<RequestResult<long>>;

public class SubmitBadgeEvidenceCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<TeacherBadgeSubmissions> submissionsRepository,
    IRepository<API.Domain.Entities.Gamification.Badges> badgesRepository,
    IRepository<UserEntity> userRepository,
    INotificationService notificationService)
    : RequestHandlerBase<SubmitBadgeEvidenceCommand, RequestResult<long>>(parameters)
{
    public override async Task<RequestResult<long>> Handle(
        SubmitBadgeEvidenceCommand request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        // Check if teacher already has a pending or approved submission for this badge
        var existingSubmission = await submissionsRepository
            .Get(x => x.TeacherId == teacherId && x.BadgeId == request.BadgeId &&
                     (x.Status == SubmissionStatus.Pending || x.Status == SubmissionStatus.Approved))
            .FirstOrDefaultAsync(cancellationToken);

        if (existingSubmission != null)
        {
            return RequestResult<long>.Failure(
                ErrorCode.ValidationError,
                $"You already have a {existingSubmission.Status.ToString().ToLower()} submission for this badge");
        }

        // Verify badge exists and is for teachers
        var badge = await badgesRepository
            .Get(x => x.ID == request.BadgeId && x.IsActive &&
                     (x.TargetRole == BadgeTargetRole.Teacher || x.TargetRole == BadgeTargetRole.Both))
            .FirstOrDefaultAsync(cancellationToken);

        if (badge == null)
        {
            return RequestResult<long>.Failure(ErrorCode.NotFound, "Badge not found or not available for teachers");
        }

        // Create badge submission (pending approval)
        var submission = new TeacherBadgeSubmissions
        {
            TeacherId = teacherId,
            BadgeId = request.BadgeId,
            EvidenceLink = request.EvidenceLink,
            SubmitterNotes = request.Notes,
            SubmittedAt = DateTime.UtcNow,
            Status = SubmissionStatus.Pending,
            CpdHoursAwarded = badge.CpdHours ?? 0
        };

        submissionsRepository.Add(submission);
        await submissionsRepository.SaveChangesAsync();

        // Notify admin(s) about new submission
        // Find admins (users with Admin role)
        var admins = await userRepository
            .Get(u => u.Role == ApplicationRole.Admin && u.IsActive)
            .ToListAsync(cancellationToken);

        var teacher = await userRepository.Get(u => u.ID == teacherId).FirstOrDefaultAsync(cancellationToken);
        var teacherName = teacher?.Name ?? "A teacher";

        foreach (var admin in admins)
        {
            await notificationService.SendBadgeSubmissionNotificationAsync(
                admin.ID,
                teacherId,
                teacherName,
                badge.Name,
                submission.ID,
                cancellationToken);
        }

        return RequestResult<long>.Success(submission.ID, "Badge evidence submitted successfully. Awaiting admin approval.");
    }
}

