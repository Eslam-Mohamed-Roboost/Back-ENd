using API.Domain.Entities.Gamification;
using API.Domain.Entities.Teacher;
using API.Domain.Entities.Users;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Services;

public interface IBadgeAwardService
{
    Task<bool> AwardBadgeToStudentAsync(long studentId, long badgeId, long? activityId = null, string? source = null, CancellationToken cancellationToken = default);
    Task<bool> AwardBadgeToTeacherAsync(long teacherId, long badgeId, string evidenceLink, string? notes = null, CancellationToken cancellationToken = default);
    Task<bool> HasBadgeAsync(long userId, long badgeId, bool isTeacher = false, CancellationToken cancellationToken = default);
}

public class BadgeAwardService : IBadgeAwardService
{
    private readonly IRepository<StudentBadges> _studentBadgesRepository;
    private readonly IRepository<TeacherBadgeSubmissions> _teacherBadgeSubmissionsRepository;
    private readonly IRepository<API.Domain.Entities.Gamification.Badges> _badgesRepository;

    public BadgeAwardService(
        IRepository<StudentBadges> studentBadgesRepository,
        IRepository<TeacherBadgeSubmissions> teacherBadgeSubmissionsRepository,
        IRepository<API.Domain.Entities.Gamification.Badges> badgesRepository)
    {
        _studentBadgesRepository = studentBadgesRepository;
        _teacherBadgeSubmissionsRepository = teacherBadgeSubmissionsRepository;
        _badgesRepository = badgesRepository;
    }

    public async Task<bool> AwardBadgeToStudentAsync(
        long studentId,
        long badgeId,
        long? activityId = null,
        string? source = null,
        CancellationToken cancellationToken = default)
    {
        // Check if student already has this badge
        var alreadyHasBadge = await _studentBadgesRepository
            .Get(x => x.StudentId == studentId && x.BadgeId == badgeId)
            .AnyAsync(cancellationToken);

        if (alreadyHasBadge)
        {
            return false; // Already has badge
        }

        // Verify badge exists and is active
        var badge = await _badgesRepository
            .Get(x => x.ID == badgeId && x.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        if (badge == null)
        {
            return false; // Badge doesn't exist or is inactive
        }

        // Award the badge
        var studentBadge = new StudentBadges
        {
            StudentId = studentId,
            BadgeId = badgeId,
            EarnedDate = DateTime.UtcNow,
            MissionId = activityId,
            AutoAwarded = true
        };

        _studentBadgesRepository.Add(studentBadge);

        return true;
    }

    public async Task<bool> AwardBadgeToTeacherAsync(
        long teacherId,
        long badgeId,
        string evidenceLink,
        string? notes = null,
        CancellationToken cancellationToken = default)
    {
        // Check if teacher already has a pending or approved submission for this badge
        var existingSubmission = await _teacherBadgeSubmissionsRepository
            .Get(x => x.TeacherId == teacherId && x.BadgeId == badgeId)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingSubmission != null && existingSubmission.Status == ReviewStatus.Approved)
        {
            return false; // Already has this badge
        }

        // Verify badge exists and is for teachers
        var badge = await _badgesRepository
            .Get(x => x.ID == badgeId && x.IsActive && 
                     (x.TargetRole == BadgeTargetRole.Teacher || x.TargetRole == BadgeTargetRole.Both))
            .FirstOrDefaultAsync(cancellationToken);

        if (badge == null)
        {
            return false; // Badge doesn't exist or not for teachers
        }

        // Create badge submission (auto-approved for system-awarded badges)
        var submission = new TeacherBadgeSubmissions
        {
            TeacherId = teacherId,
            BadgeId = badgeId,
            EvidenceLink = evidenceLink,
            SubmitterNotes = notes ?? "Automatically awarded upon completion",
            SubmittedAt = DateTime.UtcNow,
            Status = ReviewStatus.Approved, // Auto-approve system awards
            ReviewedAt = DateTime.UtcNow,
            CpdHoursAwarded = badge.CpdHours ?? 0
        };

        _teacherBadgeSubmissionsRepository.Add(submission);

        return true;
    }

    public async Task<bool> HasBadgeAsync(
        long userId,
        long badgeId,
        bool isTeacher = false,
        CancellationToken cancellationToken = default)
    {
        if (isTeacher)
        {
            return await _teacherBadgeSubmissionsRepository
                .Get(x => x.TeacherId == userId && x.BadgeId == badgeId && x.Status == ReviewStatus.Approved)
                .AnyAsync(cancellationToken);
        }
        else
        {
            return await _studentBadgesRepository
                .Get(x => x.StudentId == userId && x.BadgeId == badgeId)
                .AnyAsync(cancellationToken);
        }
    }
}

