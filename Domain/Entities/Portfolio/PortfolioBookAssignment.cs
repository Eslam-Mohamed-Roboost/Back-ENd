using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.Portfolio;

[Table("PortfolioBookAssignments", Schema = "Portfolio")]
public class PortfolioBookAssignment : BaseEntity
{
    public long StudentId { get; set; }
    public long SubjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string? Grade { get; set; }
}
