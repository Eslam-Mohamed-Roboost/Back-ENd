using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.General;

[Table("Subjects", Schema = "General")]
public class Subjects : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; } = true;
}
