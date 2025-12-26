using API.Helpers.Attributes;
using System.Text.Json.Serialization;

namespace API.Application.Features.Admin.Missions.DTOs;

public class CreateMissionResourceDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long MissionId { get; set; }
    public string Type { get; set; } = string.Empty; // "video", "article", "interactive", "pdf"
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Order { get; set; } = 0;
    public bool IsRequired { get; set; } = false;
}

public class UpdateMissionResourceDto
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Order { get; set; } = 0;
    public bool IsRequired { get; set; } = false;
}

public class MissionResourceDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    [JsonConverter(typeof(LongAsStringConverter))]
    public long MissionId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Order { get; set; }
    public bool IsRequired { get; set; }
}

