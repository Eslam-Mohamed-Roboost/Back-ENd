using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.Teacher;

[Table("TeacherSubjects", Schema = "Teacher")]
public class TeacherSubjects : BaseEntity
{
    public long TeacherId { get; set; }
    public long SubjectId { get; set; }
    public int Grade { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}
