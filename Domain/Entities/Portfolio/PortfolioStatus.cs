using System.ComponentModel.DataAnnotations.Schema;
using API.Domain.Enums;

namespace API.Domain.Entities.Portfolio;

[Table("PortfolioStatus", Schema = "Portfolio")]
public class PortfolioStatus : BaseEntity
{
    public long StudentId { get; set; }
    public long SubjectId { get; set; }
    public ReviewStatus Status { get; set; } = ReviewStatus.Pending;
    public long? LastReviewedBy { get; set; }
    public DateTime? LastReviewedAt { get; set; }
}
