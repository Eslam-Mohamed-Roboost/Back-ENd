using System.ComponentModel.DataAnnotations.Schema;
using API.Domain.Enums;

namespace API.Domain.Entities.Portfolio;

[Table("PortfolioFiles", Schema = "Portfolio")]
public class PortfolioFiles : BaseEntity
{
    public long StudentId { get; set; }
    public long SubjectId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public FileType FileType { get; set; }
    public long FileSize { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string? PreviewUrl { get; set; }
    public string DownloadUrl { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    
    // Review fields
    public string? Status { get; set; } = "Pending"; // Pending, Reviewed, NeedsRevision
    public long? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? RevisionNotes { get; set; }
    
    // Navigation properties
    public User Student { get; set; } = null!;
    public Domain.Entities.General.Subjects Subject { get; set; } = null!;
}
