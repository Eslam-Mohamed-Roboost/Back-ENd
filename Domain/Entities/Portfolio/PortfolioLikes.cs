using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.Portfolio;

[Table("PortfolioLikes", Schema = "Portfolio")]
public class PortfolioLikes : BaseEntity
{
    public long TeacherId { get; set; }
    public long StudentId { get; set; }
    public long? SubjectId { get; set; }
}
