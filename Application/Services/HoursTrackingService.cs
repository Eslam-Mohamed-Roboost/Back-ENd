using API.Domain.Entities.General;
using API.Domain.Entities.Gamification;
using API.Domain.Entities.Missions;
using API.Domain.Entities.Teacher;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Services;

public interface IHoursTrackingService
{
    Task<decimal> RecordLearningHoursAsync(long studentId, ActivityLogType activityType, long activityId, decimal hours, CancellationToken cancellationToken = default);
    Task<decimal> RecordCpdHoursAsync(long teacherId, long moduleId, decimal hours, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalLearningHoursAsync(long studentId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalCpdHoursAsync(long teacherId, CancellationToken cancellationToken = default);
    Task<decimal> CalculateMissionHoursAsync(long missionId, CancellationToken cancellationToken = default);
    Task<decimal> CalculateChallengeHoursAsync(long challengeId, CancellationToken cancellationToken = default);
}

public class HoursTrackingService : IHoursTrackingService
{
    private readonly IRepository<LearningHours> _learningHoursRepository;
    private readonly IRepository<TeacherCpdProgress> _cpdProgressRepository;
    private readonly IRepository<API.Domain.Entities.Missions.Missions> _missionsRepository;
    private readonly IRepository<Challenges> _challengesRepository;
    private readonly IRepository<StudentLevels> _studentLevelsRepository;

    public HoursTrackingService(
        IRepository<LearningHours> learningHoursRepository,
        IRepository<TeacherCpdProgress> cpdProgressRepository,
        IRepository<API.Domain.Entities.Missions.Missions> missionsRepository,
        IRepository<Challenges> challengesRepository,
        IRepository<StudentLevels> studentLevelsRepository)
    {
        _learningHoursRepository = learningHoursRepository;
        _cpdProgressRepository = cpdProgressRepository;
        _missionsRepository = missionsRepository;
        _challengesRepository = challengesRepository;
        _studentLevelsRepository = studentLevelsRepository;
    }

    public async Task<decimal> RecordLearningHoursAsync(
        long studentId,
        ActivityLogType activityType,
        long activityId,
        decimal hours,
        CancellationToken cancellationToken = default)
    {
        if (hours <= 0)
        {
            return 0;
        }

        // Check if hours already recorded for this activity
        var existing = await _learningHoursRepository
            .Get(x => x.StudentId == studentId && 
                     x.ActivityType == activityType && 
                     x.ActivityId == activityId)
            .FirstOrDefaultAsync(cancellationToken);

        if (existing != null)
        {
            return 0; // Already recorded
        }

        // Record the hours
        var learningHours = new LearningHours
        {
            StudentId = studentId,
            ActivityType = activityType,
            ActivityId = activityId,
            HoursEarned = hours,
            ActivityDate = DateTime.UtcNow
        };

        _learningHoursRepository.Add(learningHours);

        // Check for level progression
        await CheckLevelProgressionAsync(studentId, cancellationToken);

        return hours;
    }

    public async Task<decimal> RecordCpdHoursAsync(
        long teacherId,
        long moduleId,
        decimal hours,
        CancellationToken cancellationToken = default)
    {
        if (hours <= 0)
        {
            return 0;
        }

        // Check if hours already recorded for this module
        var existing = await _cpdProgressRepository
            .Get(x => x.TeacherId == teacherId && 
                     x.ModuleId == moduleId && 
                     x.HoursEarned > 0)
            .FirstOrDefaultAsync(cancellationToken);

        if (existing != null)
        {
            return 0; // Already recorded hours for this module
        }

        // Update or create CPD progress
        var progress = await _cpdProgressRepository
            .Get(x => x.TeacherId == teacherId && x.ModuleId == moduleId)
            .FirstOrDefaultAsync(cancellationToken);

        if (progress == null)
        {
            // Create new progress record
            progress = new TeacherCpdProgress
            {
                TeacherId = teacherId,
                ModuleId = moduleId,
                Status = ProgressStatus.Completed,
                HoursEarned = hours,
                CompletedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            _cpdProgressRepository.Add(progress);
        }
        else
        {
            // Update existing progress
            progress.HoursEarned = hours;
            progress.Status = ProgressStatus.Completed;
            progress.CompletedAt = DateTime.UtcNow;
            progress.UpdatedAt = DateTime.UtcNow;
            _cpdProgressRepository.Update(progress);
        }

        await _cpdProgressRepository.SaveChangesAsync();
        return hours;
    }

    public async Task<decimal> GetTotalLearningHoursAsync(
        long studentId,
        CancellationToken cancellationToken = default)
    {
        var total = await _learningHoursRepository
            .Get(x => x.StudentId == studentId)
            .SumAsync(x => x.HoursEarned, cancellationToken);

        return total;
    }

    public async Task<decimal> GetTotalCpdHoursAsync(
        long teacherId,
        CancellationToken cancellationToken = default)
    {
        var total = await _cpdProgressRepository
            .Get(x => x.TeacherId == teacherId && x.HoursEarned != null)
            .SumAsync(x => x.HoursEarned ?? 0, cancellationToken);

        return total;
    }

    public async Task<decimal> CalculateMissionHoursAsync(
        long missionId,
        CancellationToken cancellationToken = default)
    {
        var mission = await _missionsRepository
            .Get(x => x.ID == missionId)
            .FirstOrDefaultAsync(cancellationToken);

        return mission?.HoursAwarded ?? 0;
    }

    public async Task<decimal> CalculateChallengeHoursAsync(
        long challengeId,
        CancellationToken cancellationToken = default)
    {
        var challenge = await _challengesRepository
            .Get(x => x.ID == challengeId)
            .FirstOrDefaultAsync(cancellationToken);

        return challenge?.HoursAwarded ?? 0;
    }

    private async Task CheckLevelProgressionAsync(
        long studentId,
        CancellationToken cancellationToken)
    {
        // Get student's total badges
        var totalHours = await GetTotalLearningHoursAsync(studentId, cancellationToken);

        // Get or create student level record
        var studentLevel = await _studentLevelsRepository
            .Get(x => x.StudentId == studentId)
            .FirstOrDefaultAsync(cancellationToken);

        if (studentLevel == null)
        {
            return; // Level record should exist
        }

        // Calculate new level based on hours (example thresholds)
        int newLevel = totalHours switch
        {
            >= 40 => 4, // Digital Leader
            >= 25 => 3, // Digital Champion
            >= 10 => 2, // Digital Explorer
            _ => 1      // Digital Scout
        };

        if (newLevel > studentLevel.CurrentLevel)
        {
            studentLevel.CurrentLevel = newLevel;
            studentLevel.LevelName = GetLevelName(newLevel);
            studentLevel.LastLevelUpDate = DateTime.UtcNow;
            studentLevel.UpdatedAt = DateTime.UtcNow;
        }
    }

    private static StudentLevelName? GetLevelName(int level)
    {
        return level switch
        {
            4 => StudentLevelName.Leader,
            3 => StudentLevelName.Champion,
            2 => StudentLevelName.Explorer,
            _ => StudentLevelName.DigitalScout
        };
    }
}

