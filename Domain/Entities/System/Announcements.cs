using System.ComponentModel.DataAnnotations.Schema;
using API.Domain.Enums;

namespace API.Domain.Entities.System;

[Table("Announcements", Schema = "System")]
public class Announcements : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public AnnouncementPriority Priority { get; set; } = AnnouncementPriority.Normal;
    public string TargetAudience { get; set; } = string.Empty; // JSONB
    public bool IsPinned { get; set; } = false;
    public bool ShowAsPopup { get; set; } = false;
    public bool SendEmail { get; set; } = false;
    public DateTime? PublishedAt { get; set; }
    public int ViewCount { get; set; } = 0;
}
