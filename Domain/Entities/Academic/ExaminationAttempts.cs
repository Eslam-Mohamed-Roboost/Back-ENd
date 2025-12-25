using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.Academic;

[Table("ExaminationAttempts", Schema = "Academic")]
public class ExaminationAttempts : BaseEntity
{
    public long ExaminationId { get; set; }
    public long StudentId { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SubmittedAt { get; set; }
    public string? Answers { get; set; } // JSON
    public decimal? Score { get; set; }
    public string Status { get; set; } = "InProgress"; // InProgress, Submitted, Graded
    public long? GradedBy { get; set; }
    public DateTime? GradedAt { get; set; }
    public int? TimeSpent { get; set; } // Time spent in minutes

    // Navigation properties
    [ForeignKey(nameof(ExaminationId))]
    public Examinations? Examination { get; set; }

    [ForeignKey(nameof(StudentId))]
    public Identity.User? Student { get; set; }

    [ForeignKey(nameof(GradedBy))]
    public Identity.User? Grader { get; set; }
}

