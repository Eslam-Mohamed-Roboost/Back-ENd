using API.Application.Features.Student.Portfolio.DTOs;
using API.Helpers.Attributes;
using System.Text.Json.Serialization;

namespace API.Application.Features.Student.Missions.DTOs;

public class MissionDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // "completed", "in-progress", "locked", "not-started"
    public int Progress { get; set; } // 0-100
    public string Badge { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public List<string> Requirements { get; set; } = new();
}

public class MissionActivityDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Type { get; set; } = string.Empty; // "video", "quiz", "reading", "interactive"
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool Completed { get; set; }
    public int Order { get; set; }
}

public class LearningResourceDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

public class MissionDetailDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Progress { get; set; }
    public string Badge { get; set; } = string.Empty;
    public List<MissionActivityDto> Activities { get; set; } = new();
    public List<LearningResourceDto> Resources { get; set; } = new();
}

public class UpdateMissionProgressRequest
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long MissionId { get; set; }
    [JsonConverter(typeof(LongAsStringConverter))]
    public long ActivityId { get; set; }
    public bool Completed { get; set; }
    public Dictionary<string, object>? ActivityData { get; set; }
}

public class MissionProgressResponse
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long MissionId { get; set; }
    public int NewProgress { get; set; } // 0-100
    public string Status { get; set; } = string.Empty; // "in-progress", "completed"
    public PortfolioBadgeDto? BadgeEarned { get; set; }
    public decimal HoursEarned { get; set; }
}
