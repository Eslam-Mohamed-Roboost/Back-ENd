using System.Text.Json.Serialization;
using API.Helpers.Attributes;

namespace API.Application.Features.Admin.BadgeSubmissions.DTOs;

public class BadgeSubmissionDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    
    [JsonConverter(typeof(LongAsStringConverter))]
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int UserRole { get; set; }
    public string? UserAvatar { get; set; }
    
    [JsonConverter(typeof(LongAsStringConverter))]
    public long BadgeId { get; set; }
    public string BadgeName { get; set; } = string.Empty;
    public string BadgeIcon { get; set; } = string.Empty;
    public string BadgeCategory { get; set; } = string.Empty;
    public decimal? CpdHours { get; set; }
    public string? EvidenceLink { get; set; }
    public string? SubmitterNotes { get; set; }
    public DateTime SubmissionDate { get; set; }
    public string Status { get; set; } = "Pending";
    
    [JsonConverter(typeof(LongAsStringConverter))]
    public long? ReviewedBy { get; set; }
    public DateTime? ReviewDate { get; set; }
    public string? ReviewNotes { get; set; }
}

public class BadgeStatisticsDto
{
    public int Total { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
    public int Pending { get; set; }
    public int ApprovalRate { get; set; }
    public int RejectionRate { get; set; }
    public List<CategoryCountDto> ByCategory { get; set; } = new();
}

public class CategoryCountDto
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ReviewBadgeSubmissionDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long ReviewedBy { get; set; }
    public string? ReviewNotes { get; set; }
}
