using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.Gamification;

[Table("QuizAttempts", Schema = "Gamification")]
public class QuizAttempts : BaseEntity
{
    public long StudentId { get; set; }
    public long MissionId { get; set; }
    public decimal Score { get; set; }
    public decimal PassScore { get; set; } = 70.00m;
    public bool IsPassed { get; set; }
    public string? Answers { get; set; } // JSONB
    public int AttemptNumber { get; set; }
}
