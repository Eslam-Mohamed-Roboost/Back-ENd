using DotNetCore.CAP;
using API.Application.Events;
using API.Application.Services;
using API.Domain.Enums;

namespace API.Application.EventHandlers;

public class MissionCompletedEventHandler : ICapSubscribe
{
    private readonly ILogger<MissionCompletedEventHandler> _logger;
    private readonly IBadgeAwardService _badgeAwardService;
    private readonly IHoursTrackingService _hoursTrackingService;

    public MissionCompletedEventHandler(
        ILogger<MissionCompletedEventHandler> logger,
        IBadgeAwardService badgeAwardService,
        IHoursTrackingService hoursTrackingService)
    {
        _logger = logger;
        _badgeAwardService = badgeAwardService;
        _hoursTrackingService = hoursTrackingService;
    }

    [CapSubscribe("mission.completed", Group = "badge.award.service")]
    public async Task AwardBadgeAndHours(MissionCompletedEvent @event)
    {
        _logger.LogInformation(
            "Processing mission completion for Student {StudentId}, Mission {MissionId}",
            @event.StudentId, @event.MissionId);

        try
        {
            // Award badge
            var badgeAwarded = await _badgeAwardService.AwardBadgeToStudentAsync(
                @event.StudentId,
                @event.BadgeId,
                @event.MissionId,
                $"Mission: {@event.MissionTitle}");

            if (badgeAwarded)
            {
                _logger.LogInformation(
                    "Badge {BadgeId} awarded to Student {StudentId} for Mission {MissionId}",
                    @event.BadgeId, @event.StudentId, @event.MissionId);
            }

            // Record learning hours
            var hoursRecorded = await _hoursTrackingService.RecordLearningHoursAsync(
                @event.StudentId,
                ActivityLogType.Completion,
                @event.MissionId,
                @event.HoursAwarded);

            if (hoursRecorded > 0)
            {
                _logger.LogInformation(
                    "{Hours} learning hours recorded for Student {StudentId}, Mission {MissionId}",
                    hoursRecorded, @event.StudentId, @event.MissionId);
            }

            // Check for speed bonus badge (complete in < 1 hour)
            if (@event.CompletionTime < TimeSpan.FromHours(1))
            {
                _logger.LogInformation(
                    "Student {StudentId} completed mission in {Minutes} minutes - eligible for speed bonus",
                    @event.StudentId, @event.CompletionTime.TotalMinutes);
                // TODO: Award "Quick Learner" badge if configured
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error processing mission completion for Student {StudentId}, Mission {MissionId}",
                @event.StudentId, @event.MissionId);
        }
    }

    [CapSubscribe("mission.completed", Group = "notification.service")]
    public async Task SendNotification(MissionCompletedEvent @event)
    {
        _logger.LogInformation(
            "Sending mission completion notification to Student {StudentId}",
            @event.StudentId);

        // TODO: Send notification through SignalR
        // TODO: Send congratulation email
        // TODO: Update student dashboard

        await Task.CompletedTask;
    }

    [CapSubscribe("mission.completed", Group = "analytics.service")]
    public async Task TrackMissionCompletion(MissionCompletedEvent @event)
    {
        _logger.LogInformation(
            "Tracking mission completion analytics: Student {StudentId}, Mission {MissionId}, Time {Minutes}min",
            @event.StudentId, @event.MissionId, @event.CompletionTime.TotalMinutes);

        // TODO: Send to analytics service
        // TODO: Update completion statistics
        // TODO: Update leaderboard if enabled

        await Task.CompletedTask;
    }
}

