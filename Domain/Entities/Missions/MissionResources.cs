using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.Missions;

[Table("MissionResources", Schema = "Missions")]
public class MissionResources : BaseEntity
{
    public long MissionId { get; set; }
    public string Type { get; set; } = string.Empty; // "video", "article", "interactive", "pdf"
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Order { get; set; } = 0;
    public bool IsRequired { get; set; } = false;
}

