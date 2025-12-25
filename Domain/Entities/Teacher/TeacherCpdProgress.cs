using System.ComponentModel.DataAnnotations.Schema;
using API.Domain.Enums;

namespace API.Domain.Entities.Teacher;

[Table("TeacherCpdProgress", Schema = "Teacher")]
public class TeacherCpdProgress : BaseEntity
{
    public long TeacherId { get; set; }
    public long ModuleId { get; set; }
    public ProgressStatus Status { get; set; } = ProgressStatus.NotStarted;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? LastAccessedAt { get; set; }
    [Column(TypeName = "jsonb")]
    public string? EvidenceFiles { get; set; } // JSONB
    public decimal? HoursEarned { get; set; }
}
