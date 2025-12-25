using System.ComponentModel.DataAnnotations.Schema;
using API.Domain.Enums;

namespace API.Domain.Entities.Exercises;

[Table("ExerciseSubmissions", Schema = "Exercises")]
public class ExerciseSubmissions : BaseEntity
{
    public long ExerciseId { get; set; }
    public long StudentId { get; set; }
    public string? SubmissionText { get; set; }
    public string? FileUrl { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public bool IsLate { get; set; } = false;
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Pending;
    
    // Grading fields
    public decimal? Grade { get; set; }
    public string? Feedback { get; set; }
    public long? GradedBy { get; set; }
    public DateTime? GradedAt { get; set; }
    
    // Navigation properties
    public Exercises Exercise { get; set; } = null!;
    public User Student { get; set; } = null!;
    public User? GradedByTeacher { get; set; }
}

