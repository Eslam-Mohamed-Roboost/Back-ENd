using System.ComponentModel.DataAnnotations.Schema;
using API.Domain.Enums;

namespace API.Domain.Entities.Teacher;

[Table("CpdModules", Schema = "Teacher")]
public class CpdModules : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationMinutes { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public string? BackgroundColor { get; set; }
    public string? VideoUrl { get; set; }
    public VideoProvider? VideoProvider { get; set; }
    public string? GuideContent { get; set; }
    public string? FormUrl { get; set; }
    public long? BadgeId { get; set; }
    public int Order { get; set; }
    public bool IsActive { get; set; } = true;
}
