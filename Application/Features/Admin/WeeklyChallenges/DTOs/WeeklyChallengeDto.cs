using System.Text.Json.Serialization;
using API.Helpers.Attributes;

namespace API.Application.Features.Admin.WeeklyChallenges.DTOs;

public class WeeklyChallengeDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public int WeekNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> ResourceLinks { get; set; } = new();
    public string? TutorialVideo { get; set; }
    public string? SubmissionFormLink { get; set; }
    public DateTime PublishDate { get; set; }
    public string Status { get; set; } = "Draft";
    public bool AutoNotify { get; set; }
}

public class CreateWeeklyChallengeDto
{
    public int WeekNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> ResourceLinks { get; set; } = new();
    public string? TutorialVideo { get; set; }
    public string? SubmissionFormLink { get; set; }
    public string Status { get; set; } = "Draft";
    public bool AutoNotify { get; set; }
}
