using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.Teacher;

[Table("TeacherClassAssignments", Schema = "Teacher")]
public class TeacherClassAssignments : BaseEntity
{
    public long TeacherId { get; set; }
    public long ClassId { get; set; }
    public long SubjectId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [ForeignKey(nameof(TeacherId))]
    public User? Teacher { get; set; }
    
    [ForeignKey(nameof(ClassId))]
    public General.Classes? Class { get; set; }
    
    [ForeignKey(nameof(SubjectId))]
    public General.Subjects? Subject { get; set; }
}

