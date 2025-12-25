using System.ComponentModel.DataAnnotations.Schema;
using API.Domain.Enums;

namespace API.Domain.Entities.Gamification;

[Table("StudentBadges", Schema = "Gamification")]
public class StudentBadges : BaseEntity
{
    public long StudentId { get; set; }
    public long BadgeId { get; set; }
    public DateTime EarnedDate { get; set; } = DateTime.UtcNow;
    public long? MissionId { get; set; }
    public bool AutoAwarded { get; set; } = true;
    public Status Status { get; set; } = Status.Pinndeing;
}

public enum Status
{
    Pinndeing = 0,
    Approved = 1,
    Rejected = 2,
}
