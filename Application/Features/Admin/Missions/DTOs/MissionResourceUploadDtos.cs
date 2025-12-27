using Microsoft.AspNetCore.Http;

namespace API.Application.Features.Admin.Missions.DTOs;

public class UploadMissionResourceRequest
{
    public long MissionId { get; set; }
    public string Type { get; set; } = string.Empty; // "video", "article", "interactive", "pdf"
    public string Title { get; set; } = string.Empty;
    public IFormFile? File { get; set; } // File to upload (optional - can also use URL)
    public string? Url { get; set; } // External URL (optional - if no file upload)
    public string? Description { get; set; }
    public int Order { get; set; } = 0;
    public bool IsRequired { get; set; } = false;
}



