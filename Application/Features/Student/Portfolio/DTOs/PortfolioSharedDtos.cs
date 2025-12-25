using API.Helpers.Attributes;
using System.Text.Json.Serialization;

namespace API.Application.Features.Student.Portfolio.DTOs;

public class PortfolioFileDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadDate { get; set; }
    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? PreviewUrl { get; set; }
    public string DownloadUrl { get; set; } = string.Empty;
}

public class ReflectionDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? Prompt { get; set; }
    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }
    public bool AutoSaved { get; set; }
}

public class TeacherFeedbackDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Comment { get; set; } = string.Empty;
    [JsonConverter(typeof(LongAsStringConverter))]
    public long? RelatedFileId { get; set; }
}

public class PortfolioStatsDto
{
    public int FilesCount { get; set; }
    public DateTime? LatestUploadDate { get; set; }
    public int FeedbackCount { get; set; }
    public int BadgesCount { get; set; }
}

public class PortfolioBadgeDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public DateTime? EarnedDate { get; set; }
    [JsonConverter(typeof(LongAsStringConverter))]
    public long? RelatedWorkId { get; set; }
    public string Category { get; set; } = string.Empty;
}
