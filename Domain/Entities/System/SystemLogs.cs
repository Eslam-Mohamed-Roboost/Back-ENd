using System.ComponentModel.DataAnnotations.Schema;
using API.Domain.Enums;
using LogLevelEnum = API.Domain.Enums.LogLevel;

namespace API.Domain.Entities.System;

[Table("SystemLogs", Schema = "System")]
public class SystemLogs : BaseEntity
{
    public LogLevelEnum Level { get; set; } = LogLevelEnum.Info;
    public long? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
    public string? StackTrace { get; set; }
}
