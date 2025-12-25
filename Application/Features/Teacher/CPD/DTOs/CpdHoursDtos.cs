using API.Helpers.Attributes;
using System.Text.Json.Serialization;

namespace API.Application.Features.Teacher.CPD.DTOs;

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
    public decimal HoursEarned { get; set; }
    public DateTime CompletedDate { get; set; }
}

