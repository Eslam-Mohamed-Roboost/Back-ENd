using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.Missions;

[Table("StudentActivityProgress", Schema = "Missions")]
public class StudentActivityProgress : BaseEntity
{
    public long StudentId { get; set; }
    public long ActivityId { get; set; }
    public bool IsCompleted { get; set; } = false;
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }
    
    
    public long? MissionId { get; set; }
}
