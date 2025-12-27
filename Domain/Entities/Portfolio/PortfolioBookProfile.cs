using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.Portfolio;

[Table("PortfolioBookProfiles", Schema = "Portfolio")]
public class PortfolioBookProfile : BaseEntity
{
    public long StudentId { get; set; }
    public long SubjectId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string GradeSection { get; set; } = string.Empty;
    public string FavoriteThings { get; set; } = string.Empty;
    public string Uniqueness { get; set; } = string.Empty;
    public string FutureDream { get; set; } = string.Empty;
}
