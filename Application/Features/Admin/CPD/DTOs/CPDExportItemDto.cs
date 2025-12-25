namespace API.Application.Features.Admin.CPD.DTOs;

public class CPDExportItemDto
{
    public string TeacherName { get; set; } = string.Empty;
    public string TeacherEmail { get; set; } = string.Empty;
    public string ModuleTitle { get; set; } = string.Empty;
    public decimal HoursEarned { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? StartedAt { get; set; }
}
