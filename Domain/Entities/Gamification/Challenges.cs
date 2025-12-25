using System.ComponentModel.DataAnnotations.Schema;
using API.Domain.Enums;

namespace API.Domain.Entities.Gamification;

[Table("Challenges", Schema = "Gamification")]
public class Challenges : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ChallengeType Type { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public int EstimatedMinutes { get; set; } = 15;
    public string? Icon { get; set; }
    public string? BackgroundColor { get; set; }
    public string? ContentUrl { get; set; }
    public int Points { get; set; } = 25;
    
    public long? BadgeId { get; set; }
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal HoursAwarded { get; set; } = 0.5m;
    
    public bool IsActive { get; set; } = true;
}
