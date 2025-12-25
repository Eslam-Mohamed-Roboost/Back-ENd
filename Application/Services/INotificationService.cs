using API.Domain.Entities.Gamification;

namespace API.Application.Services;

public interface INotificationService
{
    /// <summary>
    /// Send notification when a badge is earned
    /// </summary>
    Task SendBadgeEarnedNotificationAsync(
        long userId,
        bool isTeacher,
        long badgeId,
        string badgeName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send notification when learning/CPD hours are awarded
    /// </summary>
    Task SendHoursAwardedNotificationAsync(
        long userId,
        bool isTeacher,
        decimal hours,
        string activityName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send notification when student levels up
    /// </summary>
    Task SendLevelUpNotificationAsync(
        long studentId,
        int newLevel,
        string levelName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send notification when mission is completed
    /// </summary>
    Task SendMissionCompletedNotificationAsync(
        long studentId,
        long missionId,
        string missionTitle,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send notification when challenge is completed
    /// </summary>
    Task SendChallengeCompletedNotificationAsync(
        long userId,
        bool isTeacher,
        long challengeId,
        string challengeTitle,
        int pointsEarned,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send notification when student uploads portfolio file
    /// </summary>
    Task SendPortfolioUploadNotificationAsync(
        long teacherId,
        long studentId,
        string studentName,
        string fileName,
        long portfolioFileId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send notification when teacher reviews portfolio
    /// </summary>
    Task SendPortfolioReviewNotificationAsync(
        long studentId,
        string teacherName,
        string fileName,
        bool needsRevision,
        string? feedbackNotes,
        long portfolioFileId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send notification when teacher submits badge evidence
    /// </summary>
    Task SendBadgeSubmissionNotificationAsync(
        long adminId,
        long teacherId,
        string teacherName,
        string badgeName,
        long submissionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send notification when admin approves/rejects badge
    /// </summary>
    Task SendBadgeApprovalNotificationAsync(
        long teacherId,
        string badgeName,
        bool approved,
        string? adminNotes,
        long badgeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send notification when exercise is assigned
    /// </summary>
    Task SendExerciseAssignedNotificationAsync(
        long studentId,
        string exerciseTitle,
        DateTime? dueDate,
        long exerciseId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send notification when student submits exercise
    /// </summary>
    Task SendExerciseSubmissionNotificationAsync(
        long teacherId,
        long studentId,
        string studentName,
        string exerciseTitle,
        long submissionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send notification when grade is published
    /// </summary>
    Task SendGradePublishedNotificationAsync(
        long studentId,
        string exerciseTitle,
        decimal grade,
        string? feedback,
        long exerciseId,
        CancellationToken cancellationToken = default);
}

