using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.Teacher;

[Table("TeacherMissions", Schema = "Teacher")]
public class TeacherMissions : BaseEntity
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

