using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.Portfolio;

[Table("PortfolioBookGoals", Schema = "Portfolio")]
public class PortfolioBookGoals : BaseEntity
{
    public long StudentId { get; set; }
    public long SubjectId { get; set; }
    public string AcademicGoal { get; set; } = string.Empty;
    public string BehavioralGoal { get; set; } = string.Empty;
    public string PersonalGrowthGoal { get; set; } = string.Empty;
    public string AchievementSteps { get; set; } = string.Empty;
    public DateTime? TargetDate { get; set; }
}
