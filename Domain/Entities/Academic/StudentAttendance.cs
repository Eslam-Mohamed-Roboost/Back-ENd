using System.ComponentModel.DataAnnotations.Schema;
using API.Domain.Enums;

namespace API.Domain.Entities.Academic;

[Table("StudentAttendance", Schema = "Academic")]
public class StudentAttendance : BaseEntity
{
    public long StudentId { get; set; }
    public long ClassId { get; set; }
    public DateTime AttendanceDate { get; set; }
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
    public long? MarkedBy { get; set; } // TeacherId who marked the attendance
    public DateTime? MarkedAt { get; set; }
    public bool IsAutomatic { get; set; } = false;
    public string? Notes { get; set; }
}

