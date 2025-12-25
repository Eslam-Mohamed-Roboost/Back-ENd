using System.ComponentModel.DataAnnotations.Schema;
using API.Domain.Enums;

namespace API.Domain.Entities.Exercises;

[Table("Exercises", Schema = "Exercises")]
public class Exercises : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ExerciseType Type { get; set; } = ExerciseType.Assignment;
    public long TeacherId { get; set; }
    public long? SubjectId { get; set; }
    public long? ClassId { get; set; }
    public DateTime? DueDate { get; set; }
    public int MaxPoints { get; set; } = 100;
    public string? Instructions { get; set; }
    public string? RubricJson { get; set; } // JSON for rubric criteria
    public bool AllowLateSubmission { get; set; } = false;
    public decimal? LatePenaltyPercentage { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public User Teacher { get; set; } = null!;
    public List<ExerciseSubmissions>? Submissions { get; set; }
}

