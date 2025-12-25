using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.Academic;

[Table("Exercises", Schema = "Academic")]
public class Exercises : BaseEntity
{
    public long TeacherId { get; set; }
    public long ClassId { get; set; }
    public long SubjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty; // Homework, Classwork, Project
    public DateTime? DueDate { get; set; }
    public decimal MaxScore { get; set; } = 100.00m;
    public string? Instructions { get; set; }
    public string? Attachments { get; set; } // JSON
    public string Status { get; set; } = "Draft"; // Draft, Published, Closed

    // Navigation properties
    [ForeignKey(nameof(TeacherId))]
    public Identity.User? Teacher { get; set; }

    [ForeignKey(nameof(ClassId))]
    public General.Classes? Class { get; set; }

    [ForeignKey(nameof(SubjectId))]
    public General.Subjects? Subject { get; set; }
}

