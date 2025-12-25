namespace API.Application.Features.Admin.Evidence.DTOs;

public class EvidenceExportItemDto
{
    public string EvidenceType { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Link { get; set; } = string.Empty;
}
