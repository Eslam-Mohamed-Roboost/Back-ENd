using System.Text.Json.Serialization;
using API.Domain.Enums;
using API.Helpers.Attributes;

namespace API.Application.Features.User.GetUsers.DTOs;

public class UserListDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ApplicationRole Role { get; set; }
    public string RoleName { get; set; }= string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? LastLogin { get; set; }
    public int BadgeCount { get; set; }
    [JsonConverter(typeof(LongAsStringConverter))]
    public long? ClassId { get; set; }
    public string? ClassName { get; set; }
}
