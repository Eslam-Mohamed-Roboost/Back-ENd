using System.ComponentModel.DataAnnotations.Schema;
using API.Domain.Enums;

namespace API.Domain.Entities.Teacher;

[Table("WeeklyChallenges", Schema = "Teacher")]
public class WeeklyChallenges : BaseEntity
{
    public int WeekNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ResourceLinks { get; set; } // JSONB
    public string? TutorialVideo { get; set; }
    public string? SubmissionFormLink { get; set; }
    public PublishStatus Status { get; set; } = PublishStatus.Draft;
    public DateTime? PublishedAt { get; set; }
    public bool AutoNotify { get; set; } = true;
}
