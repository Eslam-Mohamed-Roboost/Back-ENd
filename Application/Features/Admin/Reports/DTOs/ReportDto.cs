namespace API.Application.Features.Admin.Reports.DTOs;

public class ReportDto
{
    public string ReportId { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public ReportParametersDto Parameters { get; set; } = new();
    public object? Data { get; set; }
}

public class ReportParametersDto
{
    public string? DateFrom { get; set; }
    public string? DateTo { get; set; }
    public string? UserType { get; set; }
    public bool IncludeCharts { get; set; }
}

public class ScheduleReportDto
{
    public string ReportType { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public List<string> Recipients { get; set; } = new();
    public string Format { get; set; } = string.Empty;
}
