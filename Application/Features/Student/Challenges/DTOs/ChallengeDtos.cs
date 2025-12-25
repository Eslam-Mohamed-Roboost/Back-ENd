using API.Helpers.Attributes;
using System.Text.Json.Serialization;

namespace API.Application.Features.Student.Challenges.DTOs;

public class ChallengeDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Difficulty { get; set; } = string.Empty; // "easy", "medium", "hard"
    public int Points { get; set; }
    public bool Completed { get; set; }
    public int ParticipantCount { get; set; }
    public List<string> Tags { get; set; } = new();
}

public class SubmitChallengeRequest
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long ChallengeId { get; set; }
    public string Answer { get; set; } = string.Empty;
    public List<IFormFile>? Attachments { get; set; }
}

public class ChallengeSubmissionResponse
{
    public bool Success { get; set; }
    public int PointsEarned { get; set; }
    public bool BadgeEarned { get; set; }
    public decimal HoursEarned { get; set; }
    public string Feedback { get; set; } = string.Empty;
}
