using API.Helpers.Attributes;
using System.Text.Json.Serialization;

namespace API.Application.Features.Teacher.Attendance.DTOs;

public class AttendanceDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    [JsonConverter(typeof(LongAsStringConverter))]
    public long StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    [JsonConverter(typeof(LongAsStringConverter))]
    public long ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public DateTime AttendanceDate { get; set; }
    public string Status { get; set; } = string.Empty; // "Present", "Absent", "Late", "Excused"
    [JsonConverter(typeof(LongAsStringConverter))]
    public long? MarkedBy { get; set; }
    public string? MarkedByName { get; set; }
    public DateTime? MarkedAt { get; set; }
    public bool IsAutomatic { get; set; }
    public string? Notes { get; set; }
}

public class ClassAttendanceDto
{
    public DateTime Date { get; set; }
    [JsonConverter(typeof(LongAsStringConverter))]
    public long ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public List<AttendanceDto> Students { get; set; } = new();
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int LateCount { get; set; }
    public int ExcusedCount { get; set; }
    public int TotalStudents { get; set; }
}

public class MarkAttendanceRequest
{
    public DateTime AttendanceDate { get; set; }
    public List<StudentAttendanceEntry> Students { get; set; } = new();
}

public class StudentAttendanceEntry
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long StudentId { get; set; }
    public string Status { get; set; } = string.Empty; // "Present", "Absent", "Late", "Excused"
    public string? Notes { get; set; }
}

public class BulkMarkAttendanceRequest
{
    public DateTime AttendanceDate { get; set; }
    [JsonConverter(typeof(LongAsStringConverter))]
    public long ClassId { get; set; }
    public string Status { get; set; } = string.Empty; // "Present", "Absent", "Late", "Excused"
    public List<long> StudentIds { get; set; } = new();
    public string? Notes { get; set; }
}

public class UpdateAttendanceRequest
{
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

