using System.ComponentModel.DataAnnotations.Schema;
using API.Domain.Enums;

namespace API.Domain.Entities.Teacher;

[Table("TeacherMissionProgress", Schema = "Teacher")]
public class TeacherMissionProgress : BaseEntity
{
    public long TeacherId { get; set; }
    public long MissionId { get; set; }
    public ProgressStatus Status { get; set; } = ProgressStatus.NotStarted;
    public int CompletedActivities { get; set; } = 0;
    public int TotalActivities { get; set; }
    public decimal ProgressPercentage { get; set; } = 0.00m;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

