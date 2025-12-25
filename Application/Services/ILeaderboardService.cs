namespace API.Application.Services;

public enum LeaderboardType
{
    TotalPoints = 1,
    BadgesEarned = 2,
    HoursLogged = 3,
    MissionsCompleted = 4,
    ChallengesWon = 5
}

public enum TimeRange
{
    Weekly = 1,
    Monthly = 2,
    AllTime = 3,
    CurrentSemester = 4
}

public record LeaderboardEntry(
    long UserId,
    string UserName,
    int Rank,
    decimal Score,
    string? Avatar,
    int? Level);

public interface ILeaderboardService
{
    /// <summary>
    /// Get student leaderboard for specified type and time range
    /// </summary>
    Task<List<LeaderboardEntry>> GetStudentLeaderboardAsync(
        LeaderboardType type,
        TimeRange range,
        int limit = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user's position in leaderboard
    /// </summary>
    Task<LeaderboardEntry?> GetUserPositionAsync(
        long userId,
        LeaderboardType type,
        TimeRange range,
        CancellationToken cancellationToken = default);
}

