namespace API.Application.Features.Admin.TeacherSubjects.DTOs;

public class TeacherSubjectMatrixDto
{
    public long TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Subjects { get; set; } = new();
    public List<string> Grades { get; set; } = new();
    public int CpdBadgesEarned { get; set; }
    public int PortfolioActivity { get; set; }
    public DateTime LastActive { get; set; }
}

public class SubjectAnalyticsDto
{
    public string Subject { get; set; } = string.Empty;
    public int TeacherCount { get; set; }
    public int StudentCount { get; set; }
    public int PortfolioCompletionRate { get; set; }
    public int CpdBadgeCompletionRate { get; set; }
    public ResourceUsageStatsDto ResourceUsage { get; set; } = new();
    public List<TeacherPerformanceDto> TopTeachers { get; set; } = new();
}

public class ResourceUsageStatsDto
{
    public int TotalResources { get; set; }
    public int DownloadsThisMonth { get; set; }
    public int UploadsThisMonth { get; set; }
    public string MostPopularResource { get; set; } = string.Empty;
}

public class TeacherPerformanceDto
{
    public string TeacherName { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public int CpdBadges { get; set; }
    public int StudentEngagement { get; set; }
    public int PortfolioReviews { get; set; }
}
