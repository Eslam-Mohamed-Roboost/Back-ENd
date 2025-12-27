using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.Portfolio;

[Table("PortfolioBookLearningStyles", Schema = "Portfolio")]
public class PortfolioBookLearningStyle : BaseEntity
{
    public long StudentId { get; set; }
    public long SubjectId { get; set; }
    public string LearnsBestBy { get; set; } = string.Empty;
    public string BestTimeToStudy { get; set; } = string.Empty;
    public string FocusConditions { get; set; } = string.Empty;
    public string HelpfulTools { get; set; } = string.Empty;
    public string Distractions { get; set; } = string.Empty;
}
