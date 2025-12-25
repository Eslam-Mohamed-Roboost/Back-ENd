using System.ComponentModel.DataAnnotations.Schema;
using API.Domain.Enums;

namespace API.Domain.Entities.General;

[Table("LearningHours", Schema = "General")]
public class LearningHours : BaseEntity
{
    public long StudentId { get; set; }
    
    public ActivityLogType ActivityType { get; set; }
    
    public long ActivityId { get; set; }
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal HoursEarned { get; set; }
    
    [Column(TypeName = "timestamp with time zone")]
    public DateTime ActivityDate { get; set; }
}

