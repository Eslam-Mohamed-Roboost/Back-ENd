using API.Helpers.Attributes;
using System.Text.Json.Serialization;

namespace API.Application.Features.Student.Goals.DTOs;

public class StudentGoalDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "daily", "weekly", "monthly", "custom"
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty; // "active", "completed", "expired"
    public int CurrentProgress { get; set; }
    public int TargetProgress { get; set; }
    public int PercentageComplete { get; set; }
    public string Category { get; set; } = string.Empty; // "missions", "badges", "portfolio", "challenges"
}

public class CreateGoalRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime EndDate { get; set; }
    public int TargetProgress { get; set; }
    public string Category { get; set; } = string.Empty;
}
