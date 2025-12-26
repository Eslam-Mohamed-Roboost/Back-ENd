using System.Text.Json.Serialization;
using API.Helpers.Attributes;

namespace API.Application.Features.Teacher.Portfolio.DTOs;

public class StudentPortfolioDto
{
    [JsonConverter(typeof(LongAsStringConverter))]

    public long StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    [JsonConverter(typeof(LongAsStringConverter))]

    public long ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int TotalFiles { get; set; }
    public int PendingFiles { get; set; }
    public int ReviewedFiles { get; set; }
    public int NeedsRevisionFiles { get; set; }
    public DateTime? LastSubmissionDate { get; set; }
    public string PortfolioStatus { get; set; } = "Pending"; // Pending, Reviewed, NeedsRevision
}

public class StudentPortfolioDetailDto
{
    [JsonConverter(typeof(LongAsStringConverter))]

    public long StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    [JsonConverter(typeof(LongAsStringConverter))]

    public long SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public List<TeacherPortfolioFileDto> Files { get; set; } = new();
}

public class TeacherPortfolioFileDto
{
    [JsonConverter(typeof(LongAsStringConverter))]

    public long Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    [JsonConverter(typeof(LongAsStringConverter))]

    public long FileSize { get; set; }
    public string DownloadUrl { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public string Status { get; set; } = "Pending";
    [JsonConverter(typeof(LongAsStringConverter))]

    public long? ReviewedBy { get; set; }
    public string? ReviewerName { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? RevisionNotes { get; set; }
}

