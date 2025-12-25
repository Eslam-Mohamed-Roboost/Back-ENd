using API.Application.Features.Student.Portfolio.DTOs;
using API.Helpers.Attributes;
using System.Text.Json.Serialization;

namespace API.Application.Features.Teacher.Portfolio.DTOs;

public class TeacherStudentSummaryDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public string PortfolioStatus { get; set; } = "pending";
    public TeacherSubmissionSummaryDto? LatestSubmission { get; set; }
}

public class TeacherSubmissionSummaryDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public string Type { get; set; } = "file"; // file/onenote/etc.
}

public class TeacherPortfolioDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; } // Synthetic: same as StudentId for now
    [JsonConverter(typeof(LongAsStringConverter))]
    public long StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public List<PortfolioFileDto> Submissions { get; set; } = new();
    public List<TeacherPortfolioCommentDto> Feedback { get; set; } = new();
    public List<PortfolioBadgeDto> Badges { get; set; } = new();
    public int Likes { get; set; }
    public bool IsLiked { get; set; }
    public DateTime? LastUpdated { get; set; }
}

public class TeacherPortfolioCommentDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    [JsonConverter(typeof(LongAsStringConverter))]
    public long TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Type { get; set; } = "comment"; // "comment" or "revision-request"
}

public class TeacherPortfolioCommentRequest
{
    public string Content { get; set; } = string.Empty;
    public string Type { get; set; } = "comment"; // "comment" or "revision-request"
}

public class TeacherPortfolioRevisionRequest
{
    public string Feedback { get; set; } = string.Empty;
}

public class TeacherAwardPortfolioBadgeRequest
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long BadgeId { get; set; }
}


