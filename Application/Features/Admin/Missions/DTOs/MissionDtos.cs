using System.Text.Json.Serialization;
using API.Helpers.Attributes;

namespace API.Application.Features.Admin.Missions.DTOs;

public class CreateMissionDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int EstimatedMinutes { get; set; } = 30;
    
    [JsonConverter(typeof(LongAsStringConverter))]
    public long BadgeId { get; set; }
}

public class UpdateMissionDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int Duration { get; set; } = 30;
    
    [JsonConverter(typeof(LongAsStringConverter))]
    public long BadgeId { get; set; }
    public int Order { get; set; }
    public bool Enabled { get; set; } = true;
}

public class MissionDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public int Number { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int EstimatedMinutes { get; set; }
    
    [JsonConverter(typeof(LongAsStringConverter))]
    public long BadgeId { get; set; }
    public string? BadgeName { get; set; }
    public int Order { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
}

