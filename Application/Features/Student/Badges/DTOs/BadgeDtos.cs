using API.Application.Features.Student.Portfolio.DTOs;
using API.Helpers.Attributes;
using System.Text.Json.Serialization;

namespace API.Application.Features.Student.Badges.DTOs;

public class StudentBadgesSummaryDto
{
    public int TotalBadges { get; set; }
    public int EarnedBadges { get; set; }
    public int LockedBadges { get; set; }
    public string CurrentLevel { get; set; } = string.Empty;
    public string NextLevel { get; set; } = string.Empty;
    public int BadgesUntilNextLevel { get; set; }
    public List<BadgeDto> Badges { get; set; } = new();
    public List<PortfolioBadgeDto> PortfolioBadges { get; set; } = new();
}

public class BadgeDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public bool Earned { get; set; }
    public DateTime? EarnDate { get; set; }
    public string Requirement { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // "mission", "portfolio", "challenge"
}

public class AwardBadgeRequest
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long StudentId { get; set; }
    [JsonConverter(typeof(LongAsStringConverter))]
    public long BadgeId { get; set; }
    public string Reason { get; set; } = string.Empty;
    [JsonConverter(typeof(LongAsStringConverter))]
    public long? RelatedEntityId { get; set; }
}
