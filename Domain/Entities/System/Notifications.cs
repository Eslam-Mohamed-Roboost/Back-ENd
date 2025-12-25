using System.ComponentModel.DataAnnotations.Schema;
using API.Domain.Enums;

namespace API.Domain.Entities.System;

[Table("Notifications", Schema = "System")]
public class Notifications : BaseEntity
{
    public long UserId { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Link { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
}
