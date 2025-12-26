using API.Helpers.Attributes;
using System.Text.Json.Serialization;

namespace API.Application.Features.Student.Progress.DTOs;

public class LearningHoursSummaryDto
{
    public decimal TotalHours { get; set; }
    public decimal ThisWeekHours { get; set; }
    public decimal ThisMonthHours { get; set; }
    public List<ActivityHoursBreakdownDto> ByActivityType { get; set; } = new();
    public List<LearningHoursEntryDto> RecentActivities { get; set; } = new();
}

public class ActivityHoursBreakdownDto
{
    public string ActivityType { get; set; } = string.Empty; // "Mission", "Challenge", "Portfolio", "Completion", "Other"
    public decimal TotalHours { get; set; }
    public int Count { get; set; }
}

public class LearningHoursEntryDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    [JsonConverter(typeof(LongAsStringConverter))]
    public long? ActivityId { get; set; }
    public decimal HoursEarned { get; set; }
    public DateTime EarnedDate { get; set; }
}

