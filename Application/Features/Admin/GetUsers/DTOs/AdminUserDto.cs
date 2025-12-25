using System.Text.Json.Serialization;
using API.Helpers.Attributes;

namespace API.Application.Features.Admin.GetUsers.DTOs;

public class AdminUserDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int Role { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int BadgesCount { get; set; }
}
