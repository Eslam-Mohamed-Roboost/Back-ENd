using System.ComponentModel.DataAnnotations.Schema;
using API.Domain.Enums;

namespace API.Domain.Entities.System;

[Table("ActivityLogs", Schema = "System")]
public class ActivityLogs : BaseEntity
{
    public long UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public ActivityLogType Type { get; set; }
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    
    public string? UserName {get; set; }
}
