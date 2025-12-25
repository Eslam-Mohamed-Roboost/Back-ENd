using API.Domain.Entities.Gamification;
using API.Domain.Entities.General;
using API.Domain.Entities.Missions;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using UserEntity = API.Domain.Entities.User;

namespace API.Application.Services;

public class LeaderboardService : ILeaderboardService
{
    private readonly IRepository<UserEntity> _userRepository;
    private readonly IRepository<StudentBadges> _studentBadgesRepository;
    private readonly IRepository<LearningHours> _learningHoursRepository;
    private readonly IRepository<StudentMissionProgress> _missionProgressRepository;
    private readonly IRepository<StudentChallenges> _challengesRepository;

    public LeaderboardService(
        IRepository<UserEntity> userRepository,
        IRepository<StudentBadges> studentBadgesRepository,
        IRepository<LearningHours> learningHoursRepository,
        IRepository<StudentMissionProgress> missionProgressRepository,
        IRepository<StudentChallenges> challengesRepository)
    {
        _userRepository = userRepository;
        _studentBadgesRepository = studentBadgesRepository;
        _learningHoursRepository = learningHoursRepository;
        _missionProgressRepository = missionProgressRepository;
        _challengesRepository = challengesRepository;
    }

    public async Task<List<LeaderboardEntry>> GetStudentLeaderboardAsync(
        LeaderboardType type,
        TimeRange range,
        int limit = 100,
        CancellationToken cancellationToken = default)
    {
        var students = await _userRepository
            .Get(u => u.Role == ApplicationRole.Student && u.IsActive)
            .ToListAsync(cancellationToken);

        var dateFilter = GetDateFilter(range);
        var leaderboardData = new List<(long UserId, string UserName, decimal Score)>();

        foreach (var student in students)
        {
            var score = await CalculateScoreAsync(student.ID, type, dateFilter, cancellationToken);
            leaderboardData.Add((student.ID, student.Name, score));
        }

        var rankedData = leaderboardData
            .OrderByDescending(x => x.Score)
            .Take(limit)
            .Select((x, index) => new LeaderboardEntry(
                x.UserId,
                x.UserName,
                index + 1,
                x.Score,
                null, // Avatar not in current schema
                null  // Level not easily accessible
            ))
            .ToList();

        return rankedData;
    }

    public async Task<LeaderboardEntry?> GetUserPositionAsync(
        long userId,
        LeaderboardType type,
        TimeRange range,
        CancellationToken cancellationToken = default)
    {
        var leaderboard = await GetStudentLeaderboardAsync(type, range, 1000, cancellationToken);
        return leaderboard.FirstOrDefault(x => x.UserId == userId);
    }

    private async Task<decimal> CalculateScoreAsync(
        long studentId,
        LeaderboardType type,
        DateTime? dateFilter,
        CancellationToken cancellationToken)
    {
        return type switch
        {
            LeaderboardType.TotalPoints => 0, // Points not in current schema
            LeaderboardType.BadgesEarned => await CountBadgesAsync(studentId, dateFilter, cancellationToken),
            LeaderboardType.HoursLogged => await SumHoursAsync(studentId, dateFilter, cancellationToken),
            LeaderboardType.MissionsCompleted => await CountMissionsAsync(studentId, dateFilter, cancellationToken),
            LeaderboardType.ChallengesWon => await CountChallengesAsync(studentId, dateFilter, cancellationToken),
            _ => 0
        };
    }

    private async Task<decimal> CountBadgesAsync(long studentId, DateTime? dateFilter, CancellationToken cancellationToken)
    {
        var query = _studentBadgesRepository.Get(b => b.StudentId == studentId);
        
        if (dateFilter.HasValue)
        {
            query = query.Where(b => b.EarnedDate >= dateFilter.Value);
        }

        return await query.CountAsync(cancellationToken);
    }

    private async Task<decimal> SumHoursAsync(long studentId, DateTime? dateFilter, CancellationToken cancellationToken)
    {
        var query = _learningHoursRepository.Get(h => h.StudentId == studentId);
        
        if (dateFilter.HasValue)
        {
            query = query.Where(h => h.CreatedAt >= dateFilter.Value);
        }

        var total = await query.SumAsync(h => (decimal?)h.HoursEarned, cancellationToken);
        return total ?? 0;
    }

    private async Task<decimal> CountMissionsAsync(long studentId, DateTime? dateFilter, CancellationToken cancellationToken)
    {
        var query = _missionProgressRepository.Get(m => 
            m.StudentId == studentId && 
            m.Status == ProgressStatus.Completed);
        
        if (dateFilter.HasValue)
        {
            query = query.Where(m => m.CompletedAt >= dateFilter.Value);
        }

        return await query.CountAsync(cancellationToken);
    }

    private async Task<decimal> CountChallengesAsync(long studentId, DateTime? dateFilter, CancellationToken cancellationToken)
    {
        var query = _challengesRepository.Get(c => c.StudentId == studentId);
        
        if (dateFilter.HasValue)
        {
            query = query.Where(c => c.CreatedAt >= dateFilter.Value);
        }

        return await query.CountAsync(cancellationToken);
    }

    private DateTime? GetDateFilter(TimeRange range)
    {
        var now = DateTime.UtcNow;
        
        return range switch
        {
            TimeRange.Weekly => now.AddDays(-7),
            TimeRange.Monthly => now.AddMonths(-1),
            TimeRange.CurrentSemester => new DateTime(now.Year, now.Month <= 6 ? 1 : 7, 1),
            TimeRange.AllTime => null,
            _ => null
        };
    }
}

