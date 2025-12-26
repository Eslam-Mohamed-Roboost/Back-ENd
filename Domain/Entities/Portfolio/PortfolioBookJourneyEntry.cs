using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.Portfolio;

[Table("PortfolioBookJourneyEntries", Schema = "Portfolio")]
public class PortfolioBookJourneyEntry : BaseEntity
{
    public long StudentId { get; set; }
    public long SubjectId { get; set; }
    public DateTime Date { get; set; }
    public string SkillsWorking { get; set; } = string.Empty;
    public string EvidenceOfLearning { get; set; } = string.Empty;
    public string HowGrown { get; set; } = string.Empty;
    public string NextSteps { get; set; } = string.Empty;
}
