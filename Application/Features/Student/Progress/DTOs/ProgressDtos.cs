using API.Helpers.Attributes;
using System.Text.Json.Serialization;

namespace API.Application.Features.Student.Progress.DTOs;

public class StudentProgressDto
{
    public int TotalPoints { get; set; }
    public string CurrentLevel { get; set; } = string.Empty;
    public int LevelProgress { get; set; } // Percentage to next level
    public List<SubjectProgressDto> SubjectProgress { get; set; } = new();
    public MissionProgressSummaryDto MissionProgress { get; set; } = new();
    public BadgeProgressDto BadgeProgress { get; set; } = new();
    public List<ActivityLogDto> RecentActivity { get; set; } = new();
}

public class SubjectProgressDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int FilesUploaded { get; set; }
    public int FeedbackReceived { get; set; }
    public int BadgesEarned { get; set; }
    public int CompletionPercentage { get; set; }
}

public class MissionProgressSummaryDto
{
    public int TotalMissions { get; set; }
    public int CompletedMissions { get; set; }
    public int InProgressMissions { get; set; }
    public int LockedMissions { get; set; }
}

public class BadgeProgressDto
{
    public int TotalBadges { get; set; }
    public int EarnedBadges { get; set; }
    public int Percentage { get; set; }
}

public class ActivityLogDto
{
    public DateTime Date { get; set; }
    public string ActivityType { get; set; } = string.Empty; // "upload", "badge_earned", "mission_completed"
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}
