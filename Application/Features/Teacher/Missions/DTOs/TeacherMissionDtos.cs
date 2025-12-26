using API.Application.Features.Student.Portfolio.DTOs;
using API.Helpers.Attributes;
using System.Text.Json.Serialization;
using API.Application.Features.Student.Missions.DTOs;

namespace API.Application.Features.Teacher.Missions.DTOs;

public class TeacherMissionDto
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

public class TeacherMissionActivityDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Type { get; set; } = string.Empty; // "video", "quiz", "reading", "interactive"
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool Completed { get; set; }
    public int Order { get; set; }
}

public class TeacherMissionDetailDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Progress { get; set; }
    public string Badge { get; set; } = string.Empty;
    public List<TeacherMissionActivityDto> Activities { get; set; } = new();
    public List<LearningResourceDto> Resources { get; set; } = new();
}

public class TeacherMissionProgressDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long MissionId { get; set; }
    public string MissionTitle { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Progress { get; set; }
    public int CompletedActivities { get; set; }
    public int TotalActivities { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class TeacherMissionsProgressSummaryDto
{
    public int TotalMissions { get; set; }
    public int CompletedMissions { get; set; }
    public int InProgressMissions { get; set; }
    public int NotStartedMissions { get; set; }
    public List<TeacherMissionProgressDto> Missions { get; set; } = new();
}

public class UpdateTeacherMissionProgressRequest
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long MissionId { get; set; }
    [JsonConverter(typeof(LongAsStringConverter))]
    public long ActivityId { get; set; }
    public bool Completed { get; set; }
    public Dictionary<string, object>? ActivityData { get; set; }
}

public class TeacherMissionProgressResponse
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long MissionId { get; set; }
    public int NewProgress { get; set; } // 0-100
    public string Status { get; set; } = string.Empty; // "in-progress", "completed"
    public PortfolioBadgeDto? BadgeEarned { get; set; }
}

