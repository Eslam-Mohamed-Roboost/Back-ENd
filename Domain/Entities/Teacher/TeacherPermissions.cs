using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.Teacher;

[Table("TeacherPermissions", Schema = "Teacher")]
public class TeacherPermissions : BaseEntity
{
    public long TeacherId { get; set; }
    public long? ClassId { get; set; }
    public long? SubjectId { get; set; }
    public bool CanCreateExercises { get; set; } = true;
    public bool CanCreateExaminations { get; set; } = true;
    public bool CanGradeOwnClasses { get; set; } = true;
    public bool CanGradeAllClasses { get; set; } = false;
    public bool CanApproveGrades { get; set; } = false;

    // Navigation properties
    [ForeignKey(nameof(TeacherId))]
    public User? Teacher { get; set; }

    [ForeignKey(nameof(ClassId))]
    public General.Classes? Class { get; set; }

    [ForeignKey(nameof(SubjectId))]
    public General.Subjects? Subject { get; set; }
}

