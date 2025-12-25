namespace API.Application.Features.Admin.Dashboard.DTOs;

public class EnhancedDashboardDto
{
    public StudentAchievementMetrics StudentAchievement { get; set; } = new();
    public TeacherCPDMetrics TeacherCPD { get; set; } = new();
    public ADEKComplianceMetrics ADEKCompliance { get; set; } = new();
    public PlatformEngagementMetrics PlatformEngagement { get; set; } = new();
    public List<AIInsightDto> AIInsights { get; set; } = new();
}

public class StudentAchievementMetrics
{
    public int TotalStudents { get; set; }
    public int Grade6Count { get; set; }
    public int Grade7Count { get; set; }
    public decimal DigitalCitizenshipProgress { get; set; } // percentage
    public decimal PortfolioQualityScore { get; set; } // 1-10 scale
    public int AtRiskCount { get; set; }
    public int TopPerformersCount { get; set; }
}

public class TeacherCPDMetrics
{
    public int TotalTeachers { get; set; }
    public int ActiveTeachers { get; set; }
    public decimal CPDHoursThisMonth { get; set; }
    public decimal TargetHoursThisMonth { get; set; }
    public decimal BadgeCompletionRate { get; set; } // percentage
    public int ResourceDownloads { get; set; }
    public string TopPerformerName { get; set; } = string.Empty;
    public decimal TopPerformerHours { get; set; }
}

public class ADEKComplianceMetrics
{
    public int TotalEvidenceItems { get; set; }
    public decimal PortfolioCompletionPercentage { get; set; }
    public decimal CPDDocumentationPercentage { get; set; }
    public int PendingReviewCount { get; set; }
    public DateTime NextDeadline { get; set; }
    public int DaysUntilDeadline { get; set; }
}

public class PlatformEngagementMetrics
{
    public int DailyActiveUsers { get; set; }
    public int TotalUsers { get; set; }
    public decimal EngagementPercentage { get; set; }
    public string PeakUsageTime { get; set; } = string.Empty;
    public string MostAccessedResource { get; set; } = string.Empty;
    public decimal OneNoteAdoptionRate { get; set; } // percentage
    public decimal BadgeAdoptionRate { get; set; } // percentage
    public decimal WeeklyTrendPercentage { get; set; } // positive or negative
}

public class AIInsightDto
{
    public string Type { get; set; } = string.Empty; // "warning", "info", "success", "alert"
    public string Icon { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int? Count { get; set; }
    public string? ActionLink { get; set; }
}

