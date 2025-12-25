using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.Academic;

[Table("Grades", Schema = "Academic")]
public class Grades : BaseEntity
{
    public long StudentId { get; set; }
    public long ClassId { get; set; }
    public long SubjectId { get; set; }
    public long? ExerciseId { get; set; }
    public long? ExaminationId { get; set; }
    public decimal Score { get; set; }
    public decimal MaxScore { get; set; }
    public decimal Percentage { get; set; }
    public string? LetterGrade { get; set; }
    public string? Term { get; set; }
    public int Year { get; set; }
    public long GradedBy { get; set; }
    public DateTime GradedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Draft"; // Draft, PendingApproval, Approved, Rejected
    public long? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey(nameof(StudentId))]
    public User? Student { get; set; }

    [ForeignKey(nameof(ClassId))]
    public General.Classes? Class { get; set; }

    [ForeignKey(nameof(SubjectId))]
    public General.Subjects? Subject { get; set; }

    [ForeignKey(nameof(ExerciseId))]
    public Exercises? Exercise { get; set; }

    [ForeignKey(nameof(ExaminationId))]
    public Examinations? Examination { get; set; }

    [ForeignKey(nameof(GradedBy))]
    public User? Grader { get; set; }

    [ForeignKey(nameof(ApprovedBy))]
    public User? Approver { get; set; }
}

