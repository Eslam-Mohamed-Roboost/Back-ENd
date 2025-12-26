using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.Teacher;

[Table("TeacherActivityProgress", Schema = "Teacher")]
public class TeacherActivityProgress : BaseEntity
{
    public long TeacherId { get; set; }
    public long ActivityId { get; set; }
    public bool IsCompleted { get; set; } = false;
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }
    public long? MissionId { get; set; }
}

