using System.ComponentModel.DataAnnotations.Schema;
using API.Domain.Enums;

namespace API.Domain.Entities.Teacher;

[Table("TeacherActivities", Schema = "Teacher")]
public class TeacherActivities : BaseEntity
{
    public long MissionId { get; set; }
    public int Number { get; set; }
    public string Title { get; set; } = string.Empty;
    public ActivityType Type { get; set; }
    public string? ContentUrl { get; set; }
    public int EstimatedMinutes { get; set; } = 10;
    public string? Instructions { get; set; }
    public int Order { get; set; }
    public bool IsRequired { get; set; } = true;
}

