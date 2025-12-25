using System.Text.Json.Serialization;
using API.Helpers.Attributes;

namespace API.Application.Features.Admin.ActivityLogs.DTOs;

public class ActivityLogDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    
    [JsonConverter(typeof(LongAsStringConverter))]
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; }
}
