namespace API.Application.Features.Admin.Dashboard.DTOs;

public class AdminDashboardDto
{
    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalBadgesEarned { get; set; }
    public int TotalMissionsCompleted { get; set; }
    public int ActiveUsersThisWeek { get; set; }
    public int PortfolioFilesUploaded { get; set; }
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
    public List<TopStudentDto> TopStudents { get; set; } = new();
}

public class RecentActivityDto
{
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class TopStudentDto
{
    public long StudentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int BadgesCount { get; set; }
    public int MissionsCompleted { get; set; }
}
