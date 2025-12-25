namespace API.Application.Features.Admin.Missions.DTOs;

public class MissionProgressOverviewDto
{
    public int TotalStudents { get; set; }
    public int StudentsStarted { get; set; }
    public int StudentsCompleted { get; set; }
    public decimal AverageCompletionRate { get; set; }
    public List<MissionProgressDto> MissionProgress { get; set; } = new();
    public List<AtRiskStudentDto> AtRiskStudents { get; set; } = new();
}

public class MissionProgressDto
{
    public long MissionId { get; set; }
    public string MissionTitle { get; set; } = string.Empty;
    public int StudentsStarted { get; set; }
    public int StudentsCompleted { get; set; }
    public decimal CompletionRate { get; set; }
}

public class AtRiskStudentDto
{
    public long StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public int MissionsStarted { get; set; }
    public int MissionsCompleted { get; set; }
    public decimal CompletionRate { get; set; }
    public DateTime? LastActivityDate { get; set; }
}

public class ClassMissionProgressDto
{
    public long ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int Grade { get; set; }
    public int TotalStudents { get; set; }
    public decimal AverageCompletionRate { get; set; }
    public List<StudentMissionProgressDto> StudentProgress { get; set; } = new();
}

public class StudentMissionProgressDto
{
    public long StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int MissionsCompleted { get; set; }
    public int TotalMissions { get; set; }
    public decimal CompletionRate { get; set; }
    public DateTime? LastActivityDate { get; set; }
}

