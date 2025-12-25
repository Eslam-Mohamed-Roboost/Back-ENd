using System.ComponentModel.DataAnnotations.Schema;
using API.Domain.Enums;

namespace API.Domain.Entities.Missions;

[Table("StudentMissionProgress", Schema = "Missions")]
public class StudentMissionProgress : BaseEntity
{
    public long StudentId { get; set; }
    public long MissionId { get; set; }
    public ProgressStatus Status { get; set; } = ProgressStatus.NotStarted;
    public int CompletedActivities { get; set; } = 0;
    public int TotalActivities { get; set; }
    public decimal ProgressPercentage { get; set; } = 0.00m;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
