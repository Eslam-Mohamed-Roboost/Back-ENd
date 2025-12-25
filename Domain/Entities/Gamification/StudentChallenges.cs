using System.ComponentModel.DataAnnotations.Schema;
using API.Domain.Enums;

namespace API.Domain.Entities.Gamification;

[Table("StudentChallenges", Schema = "Gamification")]
public class StudentChallenges : BaseEntity
{
    public long StudentId { get; set; }
    public long ChallengeId { get; set; }
    public ProgressStatus Status { get; set; } = ProgressStatus.NotStarted;
    public int? Score { get; set; }
    public int PointsEarned { get; set; } = 0;
    public DateTime? CompletedAt { get; set; }
}
