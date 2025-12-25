using System.Text.Json.Serialization;
using API.Helpers.Attributes;

namespace API.Application.Features.Admin.CPD.DTOs;

public class CPDStatisticsDto
{
    public int TotalTeachers { get; set; }
    public int ActiveTeachers { get; set; }
    public int TotalCPDHours { get; set; }
    public int AverageHoursPerTeacher { get; set; }
    public string TopPerformer { get; set; } = string.Empty;
    public int TopPerformerHours { get; set; }
}

public class TeacherCPDProgressDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int BadgeCount { get; set; }
    public int CpdHours { get; set; }
    public DateTime? LastBadgeDate { get; set; }
    public List<string> Categories { get; set; } = new();
}

public class CPDByMonthDto
{
    public string Month { get; set; } = string.Empty;
    public int Hours { get; set; }
}

public class CPDCategoryDto
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
}
