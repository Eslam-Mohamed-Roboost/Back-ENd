namespace API.Application.Features.Admin.Portfolio.DTOs;

public class PortfolioCompletionStatsDto
{
    public int TotalStudents { get; set; }
    public int ActivePortfolios { get; set; }
    public int CompletionRate { get; set; }
    public List<ClassPortfolioStatsDto> ByClass { get; set; } = new();
    public List<SubjectPortfolioStatsDto> BySubject { get; set; } = new();
    public List<PortfolioUpdateDto> RecentUpdates { get; set; } = new();
}

public class ClassPortfolioStatsDto
{
    public string ClassName { get; set; } = string.Empty;
    public int Grade { get; set; }
    public int TotalStudents { get; set; }
    public int ActivePortfolios { get; set; }
    public int CompletionRate { get; set; }
}

public class SubjectPortfolioStatsDto
{
    public string SubjectName { get; set; } = string.Empty;
    public int TotalSubmissions { get; set; }
    public int ActiveStudents { get; set; }
    public int RecentActivity { get; set; }
}

public class PortfolioUpdateDto
{
    public long StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string UpdateType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public int ItemCount { get; set; }
}
