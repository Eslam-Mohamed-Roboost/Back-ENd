using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.Academic;

[Table("ExerciseSubmissions", Schema = "Academic")]
public class ExerciseSubmissions : BaseEntity
{
    public long ExerciseId { get; set; }
    public long StudentId { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public string? Content { get; set; }
    public string? Attachments { get; set; } // JSON
    public string Status { get; set; } = "Submitted"; // Submitted, Late, Graded
    public decimal? Score { get; set; }
    public string? Feedback { get; set; }
    public long? GradedBy { get; set; }
    public DateTime? GradedAt { get; set; }

    // Navigation properties
    [ForeignKey(nameof(ExerciseId))]
    public Exercises? Exercise { get; set; }

    [ForeignKey(nameof(StudentId))]
    public Identity.User? Student { get; set; }

    [ForeignKey(nameof(GradedBy))]
    public Identity.User? Grader { get; set; }
}

