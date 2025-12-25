using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.Missions;

[Table("Missions", Schema = "Missions")]
public class Missions : BaseEntity
{
    public int Number { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int EstimatedMinutes { get; set; } = 30;
    public long BadgeId { get; set; }
    public int Order { get; set; }
    public bool IsEnabled { get; set; } = true;
}
