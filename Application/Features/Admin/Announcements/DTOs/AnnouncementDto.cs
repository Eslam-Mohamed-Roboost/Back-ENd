using System.Text.Json.Serialization;
using API.Helpers.Attributes;

namespace API.Application.Features.Admin.Announcements.DTOs;

public class AnnouncementDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Priority { get; set; }
    public string PriorityName { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
    public bool ShowAsPopup { get; set; }
    public bool SendEmail { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateAnnouncementDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Priority { get; set; } = 1;
    public string TargetAudience { get; set; } = "[]";
    public bool IsPinned { get; set; }
    public bool ShowAsPopup { get; set; }
    public bool SendEmail { get; set; }
    public bool PublishNow { get; set; }
}
