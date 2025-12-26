using API.Helpers.Attributes;
using System.Text.Json.Serialization;

namespace API.Application.Features.Student.Attendance.DTOs;

public class StudentAttendanceHistoryDto
{
    public DateTime Date { get; set; }
    [JsonConverter(typeof(LongAsStringConverter))]
    public long ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // "Present", "Absent", "Late", "Excused"
    public bool IsAutomatic { get; set; }
    public string? Notes { get; set; }
}

public class AttendanceStatisticsDto
{
    public int TotalDays { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public int LateDays { get; set; }
    public int ExcusedDays { get; set; }
    public decimal AttendancePercentage { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public DateTime? LastAttendanceDate { get; set; }
    public List<StudentAttendanceHistoryDto> RecentHistory { get; set; } = new();
}

