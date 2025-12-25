using API.Domain.Enums;
using System.Text.Json.Serialization;
using API.Helpers.Attributes;

namespace API.Application.Features.Admin.Badges.DTOs;

public class BadgeDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public int Category { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int TargetRole { get; set; }
    public string TargetRoleName { get; set; } = string.Empty;
    public decimal? CpdHours { get; set; }
    public bool IsActive { get; set; }
    public int EarnedCount { get; set; }
}

public class CreateBadgeDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public BadgeCategory Category { get; set; }
    public BadgeTargetRole TargetRole { get; set; }
    public decimal? CpdHours { get; set; }
}

public class UpdateBadgeDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public int Category { get; set; }
    public int TargetRole { get; set; }
    public decimal? CpdHours { get; set; }
    public bool IsActive { get; set; }
}
