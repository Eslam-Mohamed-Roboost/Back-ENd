using System.ComponentModel.DataAnnotations.Schema;
using API.Domain.Enums;

namespace API.Domain.Entities.Portfolio;

[Table("TeacherFeedback", Schema = "Portfolio")]
public class TeacherFeedback : BaseEntity
{
    public long StudentId { get; set; }
    public long TeacherId { get; set; }
    public long? SubjectId { get; set; }
    public long? FileId { get; set; }
    public string Comment { get; set; } = string.Empty;
    public FeedbackType Type { get; set; }
}
