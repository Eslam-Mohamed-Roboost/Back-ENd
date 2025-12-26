using API.Helpers.Attributes;
using System.Text.Json.Serialization;

namespace API.Application.Features.Teacher.Lounge.DTOs;

public class LeaderboardEntryDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long TeacherId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public int Rank { get; set; }
}

public class CurrentUserRankDto
{
    public int Rank { get; set; }
    public string Value { get; set; } = string.Empty;
}

public class TeachersLoungeStatsDto
{
    public decimal TotalCpdHours { get; set; }
    public decimal CpdHoursChangePercent { get; set; }
    public int BadgesAwarded { get; set; }
    public decimal BadgesChangePercent { get; set; }
    public int ActiveTeachers { get; set; }
    public decimal ActiveTeachersChangePercent { get; set; }
    public decimal EngagementRate { get; set; }
    public decimal EngagementChangePercent { get; set; }
}

public class TeachersLoungeDto
{
    public List<LeaderboardEntryDto> CpdLeaders { get; set; } = new();
    public CurrentUserRankDto? CurrentUserCpdRank { get; set; }
    public List<LeaderboardEntryDto> BadgeLeaders { get; set; } = new();
    public CurrentUserRankDto? CurrentUserBadgeRank { get; set; }
    public TeachersLoungeStatsDto Stats { get; set; } = new();
}

