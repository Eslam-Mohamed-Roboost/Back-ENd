using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.Academic;

[Table("Examinations", Schema = "Academic")]
public class Examinations : BaseEntity
{
    public long TeacherId { get; set; }
    public long ClassId { get; set; }
    public long SubjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty; // Quiz, Test, Exam
    public DateTime? ScheduledDate { get; set; }
    public int? Duration { get; set; } // Duration in minutes
    public decimal MaxScore { get; set; } = 100.00m;
    public string? Instructions { get; set; }
    public string? Questions { get; set; } // JSON
    public string Status { get; set; } = "Draft"; // Draft, Scheduled, InProgress, Completed

    // Navigation properties
    [ForeignKey(nameof(TeacherId))]
    public User? Teacher { get; set; }

    [ForeignKey(nameof(ClassId))]
    public General.Classes? Class { get; set; }

    [ForeignKey(nameof(SubjectId))]
    public General.Subjects? Subject { get; set; }
}

