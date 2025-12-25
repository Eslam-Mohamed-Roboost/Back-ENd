using API.Helpers.Attributes;
using System.Text.Json.Serialization;

namespace API.Application.Features.Student.Activity.DTOs;

public class ActivityStreakDto
{
    public int CurrentStreak { get; set; } // Days
    public int LongestStreak { get; set; }
    public DateTime LastActivityDate { get; set; }
    public bool IsActiveToday { get; set; }
    public List<DateTime> ActivityCalendar { get; set; } = new(); // Last 30 days
}

public class PointsSummaryDto
{
    public int TotalPoints { get; set; }
    public int PointsThisWeek { get; set; }
    public int PointsThisMonth { get; set; }
    public string CurrentLevel { get; set; } = string.Empty;
    public int PointsToNextLevel { get; set; }
    public List<PointsHistoryDto> RecentEarnings { get; set; } = new();
    public PointsBreakdownDto Breakdown { get; set; } = new();
}

public class PointsHistoryDto
{
    public DateTime Date { get; set; }
    public int Points { get; set; }
    public string Source { get; set; } = string.Empty; // "Mission Completed", "Badge Earned", etc.
    public string Description { get; set; } = string.Empty;
}

public class PointsBreakdownDto
{
    public int FromMissions { get; set; }
    public int FromChallenges { get; set; }
    public int FromBadges { get; set; }
    public int FromPortfolio { get; set; }
    public int FromStreak { get; set; }
}

public class AwardPointsRequest
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long StudentId { get; set; }
    public int Points { get; set; }
    public string Source { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
