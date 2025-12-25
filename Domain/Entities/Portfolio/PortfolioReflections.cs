using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.Portfolio;

[Table("PortfolioReflections", Schema = "Portfolio")]
public class PortfolioReflections : BaseEntity
{
    public long StudentId { get; set; }
    public long SubjectId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Prompt { get; set; }
    public bool IsAutoSaved { get; set; } = false;
}
