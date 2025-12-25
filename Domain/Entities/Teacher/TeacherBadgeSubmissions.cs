using System.ComponentModel.DataAnnotations.Schema;
using API.Domain.Enums;

namespace API.Domain.Entities.Teacher;

[Table("TeacherBadgeSubmissions", Schema = "Teacher")]
public class TeacherBadgeSubmissions : BaseEntity
{
    public long TeacherId { get; set; }
    public long BadgeId { get; set; }
    public string EvidenceLink { get; set; } = string.Empty;
    public string? SubmitterNotes { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Pending;
    public long? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNotes { get; set; }
    public decimal? CpdHoursAwarded { get; set; }
}
