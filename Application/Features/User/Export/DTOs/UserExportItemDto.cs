using System.Text.Json.Serialization;
using API.Helpers.Attributes;

namespace API.Application.Features.User.Export.DTOs;

public class UserExportItemDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? LastLogin { get; set; }
    public int BadgeCount { get; set; }
}
