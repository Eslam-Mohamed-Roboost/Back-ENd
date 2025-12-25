using DotNetCore.CAP;
using API.Application.Events;
using API.Application.Services;
using API.Domain.Enums;

namespace API.Application.EventHandlers;

public class ChallengeCompletedEventHandler : ICapSubscribe
{
    private readonly ILogger<ChallengeCompletedEventHandler> _logger;
    private readonly IBadgeAwardService _badgeAwardService;
    private readonly IHoursTrackingService _hoursTrackingService;

    public ChallengeCompletedEventHandler(
        ILogger<ChallengeCompletedEventHandler> logger,
        IBadgeAwardService badgeAwardService,
        IHoursTrackingService hoursTrackingService)
    {
        _logger = logger;
        _badgeAwardService = badgeAwardService;
        _hoursTrackingService = hoursTrackingService;
    }

    [CapSubscribe("challenge.completed", Group = "badge.award.service")]
    public async Task AwardBadgeAndHours(ChallengeCompletedEvent @event)
    {
        _logger.LogInformation(
            "Processing challenge completion for User {UserId} (IsTeacher: {IsTeacher}), Challenge {ChallengeId}",
            @event.UserId, @event.IsTeacher, @event.ChallengeId);

        try
        {
            // Award badge if configured
            if (@event.BadgeId.HasValue)
            {
                bool badgeAwarded;
                
                if (@event.IsTeacher)
                {
                    badgeAwarded = await _badgeAwardService.AwardBadgeToTeacherAsync(
                        @event.UserId,
                        @event.BadgeId.Value,
                        $"/challenges/{@event.ChallengeId}",
                        $"Challenge: {@event.ChallengeTitle}");
                }
                else
                {
                    badgeAwarded = await _badgeAwardService.AwardBadgeToStudentAsync(
                        @event.UserId,
                        @event.BadgeId.Value,
                        @event.ChallengeId,
                        $"Challenge: {@event.ChallengeTitle}");
                }

                if (badgeAwarded)
                {
                    _logger.LogInformation(
                        "Badge {BadgeId} awarded to User {UserId} for Challenge {ChallengeId}",
                        @event.BadgeId.Value, @event.UserId, @event.ChallengeId);
                }
            }

            // Record hours
            if (@event.IsTeacher)
            {
                // TODO: Implement teacher CPD module tracking
                // For now, this would be through CPD progress
                _logger.LogInformation(
                    "Teacher {TeacherId} completed challenge - CPD hours should be recorded separately",
                    @event.UserId);
            }
            else
            {
                var hoursRecorded = await _hoursTrackingService.RecordLearningHoursAsync(
                    @event.UserId,
                    ActivityLogType.Completion,
                    @event.ChallengeId,
                    @event.HoursAwarded);

                if (hoursRecorded > 0)
                {
                    _logger.LogInformation(
                        "{Hours} learning hours recorded for Student {StudentId}, Challenge {ChallengeId}",
                        hoursRecorded, @event.UserId, @event.ChallengeId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error processing challenge completion for User {UserId}, Challenge {ChallengeId}",
                @event.UserId, @event.ChallengeId);
        }
    }

    [CapSubscribe("challenge.completed", Group = "notification.service")]
    public async Task SendNotification(ChallengeCompletedEvent @event)
    {
        _logger.LogInformation(
            "Sending challenge completion notification to User {UserId}",
            @event.UserId);

        // TODO: Send notification through SignalR
        // TODO: Send celebration modal trigger
        // TODO: Update user dashboard

        await Task.CompletedTask;
    }

    [CapSubscribe("challenge.completed", Group = "analytics.service")]
    public async Task TrackChallengeCompletion(ChallengeCompletedEvent @event)
    {
        _logger.LogInformation(
            "Tracking challenge completion analytics: User {UserId}, Challenge {ChallengeId}",
            @event.UserId, @event.ChallengeId);

        // TODO: Send to analytics service
        // TODO: Update completion statistics

        await Task.CompletedTask;
    }
}

