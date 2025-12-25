using System.ComponentModel.DataAnnotations.Schema;
using API.Domain.Enums;

namespace API.Domain.Entities.Gamification;

[Table("StudentLevels", Schema = "Gamification")]
public class StudentLevels : BaseEntity
{
    public long StudentId { get; set; }
    public int CurrentLevel { get; set; } = 1;
    public StudentLevelName? LevelName { get; set; }
    public int BadgesEarned { get; set; } = 0;
    public int? NextLevelBadgeCount { get; set; }
    public string? LevelIcon { get; set; }
    public DateTime? LastLevelUpDate { get; set; }
}
