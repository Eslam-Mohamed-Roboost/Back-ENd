using API.Domain.Entities.System;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;

namespace API.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IRepository<Notifications> _notificationsRepository;

    public NotificationService(IRepository<Notifications> notificationsRepository)
    {
        _notificationsRepository = notificationsRepository;
    }

    public async Task SendBadgeEarnedNotificationAsync(
        long userId,
        bool isTeacher,
        long badgeId,
        string badgeName,
        CancellationToken cancellationToken = default)
    {
        var notification = new Notifications
        {
            UserId = userId,
            Type = NotificationType.BadgeEarned,
            Title = "🏆 New Badge Earned!",
            Message = $"Congratulations! You've earned the '{badgeName}' badge!",
            Link = $"/badges/{badgeId}",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _notificationsRepository.Add(notification);
        await _notificationsRepository.SaveChangesAsync();
    }

    public async Task SendHoursAwardedNotificationAsync(
        long userId,
        bool isTeacher,
        decimal hours,
        string activityName,
        CancellationToken cancellationToken = default)
    {
        var hourType = isTeacher ? "CPD" : "learning";
        var notification = new Notifications
        {
            UserId = userId,
            Type = NotificationType.Announcement, // Using Announcement as general notification type
            Title = $"⏱️ {hours} {hourType} hours earned!",
            Message = $"You've earned {hours} {hourType} hours for completing '{activityName}'.",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _notificationsRepository.Add(notification);
        await _notificationsRepository.SaveChangesAsync();
    }

    public async Task SendLevelUpNotificationAsync(
        long studentId,
        int newLevel,
        string levelName,
        CancellationToken cancellationToken = default)
    {
        var notification = new Notifications
        {
            UserId = studentId,
            Type = NotificationType.LevelUp,
            Title = "🎉 Level Up!",
            Message = $"Congratulations! You've reached level {newLevel}: {levelName}!",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _notificationsRepository.Add(notification);
        await _notificationsRepository.SaveChangesAsync();
    }

    public async Task SendMissionCompletedNotificationAsync(
        long studentId,
        long missionId,
        string missionTitle,
        CancellationToken cancellationToken = default)
    {
        var notification = new Notifications
        {
            UserId = studentId,
            Type = NotificationType.MissionComplete,
            Title = "✅ Mission Completed!",
            Message = $"Great job! You've completed the mission: {missionTitle}",
            Link = $"/student/missions/{missionId}",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _notificationsRepository.Add(notification);
        await _notificationsRepository.SaveChangesAsync();
    }

    public async Task SendChallengeCompletedNotificationAsync(
        long userId,
        bool isTeacher,
        long challengeId,
        string challengeTitle,
        int pointsEarned,
        CancellationToken cancellationToken = default)
    {
        var notification = new Notifications
        {
            UserId = userId,
            Type = NotificationType.NewChallenge,
            Title = "⚡ Challenge Completed!",
            Message = $"You've completed '{challengeTitle}' and earned {pointsEarned} points!",
            Link = $"/challenges/{challengeId}",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _notificationsRepository.Add(notification);
        await _notificationsRepository.SaveChangesAsync();
    }

    public async Task SendPortfolioUploadNotificationAsync(
        long teacherId,
        long studentId,
        string studentName,
        string fileName,
        long portfolioFileId,
        CancellationToken cancellationToken = default)
    {
        var notification = new Notifications
        {
            UserId = teacherId,
            Type = NotificationType.Announcement,
            Title = "📁 New Portfolio Upload",
            Message = $"{studentName} has uploaded a new file: {fileName}",
            Link = $"/teacher/portfolio/{studentId}",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _notificationsRepository.Add(notification);
        await _notificationsRepository.SaveChangesAsync();
    }

    public async Task SendPortfolioReviewNotificationAsync(
        long studentId,
        string teacherName,
        string fileName,
        bool needsRevision,
        string? feedbackNotes,
        long portfolioFileId,
        CancellationToken cancellationToken = default)
    {
        var status = needsRevision ? "needs revision" : "has been reviewed";
        var icon = needsRevision ? "🔄" : "✅";
        
        var notification = new Notifications
        {
            UserId = studentId,
            Type = NotificationType.Announcement,
            Title = $"{icon} Portfolio Feedback",
            Message = $"{teacherName} reviewed '{fileName}' - {status}. {feedbackNotes}",
            Link = $"/student/portfolio",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _notificationsRepository.Add(notification);
        await _notificationsRepository.SaveChangesAsync();
    }

    public async Task SendBadgeSubmissionNotificationAsync(
        long adminId,
        long teacherId,
        string teacherName,
        string badgeName,
        long submissionId,
        CancellationToken cancellationToken = default)
    {
        var notification = new Notifications
        {
            UserId = adminId,
            Type = NotificationType.Announcement,
            Title = "🏅 New Badge Submission",
            Message = $"{teacherName} has submitted evidence for the '{badgeName}' badge",
            Link = $"/admin/badges/submissions/{submissionId}",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _notificationsRepository.Add(notification);
        await _notificationsRepository.SaveChangesAsync();
    }

    public async Task SendBadgeApprovalNotificationAsync(
        long teacherId,
        string badgeName,
        bool approved,
        string? adminNotes,
        long badgeId,
        CancellationToken cancellationToken = default)
    {
        var status = approved ? "approved" : "rejected";
        var icon = approved ? "✅" : "❌";
        var message = approved 
            ? $"Congratulations! Your '{badgeName}' badge has been approved!" 
            : $"Your '{badgeName}' badge submission was not approved. {adminNotes}";
        
        var notification = new Notifications
        {
            UserId = teacherId,
            Type = approved ? NotificationType.BadgeEarned : NotificationType.Announcement,
            Title = $"{icon} Badge {status}",
            Message = message,
            Link = $"/teacher/badges",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _notificationsRepository.Add(notification);
        await _notificationsRepository.SaveChangesAsync();
    }

    public async Task SendExerciseAssignedNotificationAsync(
        long studentId,
        string exerciseTitle,
        DateTime? dueDate,
        long exerciseId,
        CancellationToken cancellationToken = default)
    {
        var dueDateText = dueDate.HasValue ? $" - Due: {dueDate.Value:MMM dd, yyyy}" : "";
        var notification = new Notifications
        {
            UserId = studentId,
            Type = NotificationType.Announcement,
            Title = "📝 New Exercise Assigned",
            Message = $"You have a new exercise: {exerciseTitle}{dueDateText}",
            Link = $"/student/exercises/{exerciseId}",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _notificationsRepository.Add(notification);
        await _notificationsRepository.SaveChangesAsync();
    }

    public async Task SendExerciseSubmissionNotificationAsync(
        long teacherId,
        long studentId,
        string studentName,
        string exerciseTitle,
        long submissionId,
        CancellationToken cancellationToken = default)
    {
        var notification = new Notifications
        {
            UserId = teacherId,
            Type = NotificationType.Announcement,
            Title = "📋 Exercise Submitted",
            Message = $"{studentName} has submitted: {exerciseTitle}",
            Link = $"/teacher/submissions/{submissionId}",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _notificationsRepository.Add(notification);
        await _notificationsRepository.SaveChangesAsync();
    }

    public async Task SendGradePublishedNotificationAsync(
        long studentId,
        string exerciseTitle,
        decimal grade,
        string? feedback,
        long exerciseId,
        CancellationToken cancellationToken = default)
    {
        var feedbackText = !string.IsNullOrEmpty(feedback) ? $" - {feedback}" : "";
        var notification = new Notifications
        {
            UserId = studentId,
            Type = NotificationType.Announcement,
            Title = "📊 Grade Published",
            Message = $"Your grade for '{exerciseTitle}' is {grade}%{feedbackText}",
            Link = $"/student/exercises/{exerciseId}",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _notificationsRepository.Add(notification);
        await _notificationsRepository.SaveChangesAsync();
    }
}

