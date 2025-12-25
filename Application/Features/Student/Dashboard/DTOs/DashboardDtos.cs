using API.Application.Features.Student.Badges.DTOs;
using API.Application.Features.Student.Missions.DTOs;
using API.Helpers.Attributes;
using System.Text.Json.Serialization;

namespace API.Application.Features.Student.Dashboard.DTOs;

public class StudentDashboardDto
{
    public StudentInfoDto StudentInfo { get; set; } = new();
    public QuickStatsDto QuickStats { get; set; } = new();
    public List<SubjectCardDto> SubjectHubs { get; set; } = new();
    public List<MissionDto> InProgressMissions { get; set; } = new();
    public List<NotificationDto> Notifications { get; set; } = new();
    public List<BadgeDto> RecentBadges { get; set; } = new();
}

public class StudentInfoDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
}

public class QuickStatsDto
{
    public int TotalBadges { get; set; }
    public int CompletedMissions { get; set; }
    public int PortfolioFiles { get; set; }
    public int Points { get; set; }
}

public class SubjectCardDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int NewFeedbackCount { get; set; }
    public int PendingTasksCount { get; set; }
}

public class NotificationDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Type { get; set; } = string.Empty; // "feedback", "badge", "mission", "challenge"
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public bool Read { get; set; }
    public string ActionUrl { get; set; } = string.Empty;
}
