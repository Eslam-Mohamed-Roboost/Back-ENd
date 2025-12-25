using System.ComponentModel.DataAnnotations.Schema;
using API.Domain.Enums;

namespace API.Domain.Entities.Gamification;

[Table("Badges", Schema = "Gamification")]
public class Badges : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public BadgeCategory Category { get; set; }
    public BadgeTargetRole TargetRole { get; set; }
    public decimal? CpdHours { get; set; }
    public bool IsActive { get; set; } = true;
}
