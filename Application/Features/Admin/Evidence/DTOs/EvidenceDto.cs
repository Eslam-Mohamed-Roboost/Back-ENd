namespace API.Application.Features.Admin.Evidence.DTOs;

public class EvidenceStatsDto
{
    public int TotalEvidenceItems { get; set; }
    public int ThisMonth { get; set; }
    public EvidenceByTypeDto ByType { get; set; } = new();
    public int PendingReview { get; set; }
}

public class EvidenceByTypeDto
{
    public int Portfolios { get; set; }
    public int Cpd { get; set; }
    public int Badges { get; set; }
}

public class EvidenceExportRequestDto
{
    public DateRangeDto DateRange { get; set; } = new();
    public List<string> Subjects { get; set; } = new();
    public List<string> EvidenceTypes { get; set; } = new();
    public string Format { get; set; } = "zip";
}

public class DateRangeDto
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}

public class EvidenceExportStatusDto
{
    public string ExportId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? DownloadUrl { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? EstimatedCompletionTime { get; set; }
}
