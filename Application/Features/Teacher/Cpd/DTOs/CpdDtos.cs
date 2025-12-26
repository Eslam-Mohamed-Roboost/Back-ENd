using System.Text.Json.Serialization;
using API.Helpers.Attributes;

namespace API.Application.Features.Teacher.Cpd.DTOs;

public class CpdModuleDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string Status { get; set; } = "not-started"; // not-started | in-progress | completed
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string BgColor { get; set; } = string.Empty;
    public string? VideoUrl { get; set; }
    public string? VideoProvider { get; set; }
    public string? GuideContent { get; set; }
    public string? FormUrl { get; set; }
    public List<string> EvidenceFiles { get; set; } = new();
    public DateTime? CompletedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? LastAccessedAt { get; set; }
}

public class CpdProgressDto
{
    public decimal HoursCompleted { get; set; }
    public decimal TargetHours { get; set; }
    public int CompletedModules { get; set; }
    public int TotalModules { get; set; }
    public DateTime? LastActivityDate { get; set; }
    public int Streak { get; set; }
}

public class TeacherDashboardDto
{
    public CpdProgressDto CpdProgress { get; set; } = new();
    public TeacherStatsDto Stats { get; set; } = new();
}

public class TeacherStatsDto
{
    public decimal CpdHours { get; set; }
    public int BadgesEarned { get; set; }
    public int ActiveStudents { get; set; }
    public int CurrentStreak { get; set; }
}

public class CpdHoursSummaryDto
{
    public decimal TotalHours { get; set; }
    public decimal ThisYearHours { get; set; }
    public decimal AnnualGoal { get; set; }
    public decimal ProgressPercentage { get; set; }
    public List<CpdHoursEntryDto> RecentActivities { get; set; } = new();
}

public class CpdHoursEntryDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    [JsonConverter(typeof(LongAsStringConverter))]
    public long ModuleId { get; set; }
    public string ModuleTitle { get; set; } = string.Empty;
    public decimal HoursEarned { get; set; }
    public DateTime CompletedDate { get; set; }
}


