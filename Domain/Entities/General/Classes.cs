using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.General;

[Table("Classes", Schema = "General")]
public class Classes : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int Grade { get; set; }
    public long? TeacherId { get; set; }
    public int StudentCount { get; set; } = 0;
    
    public IEnumerable<User> Students { get; set; } = new List<User>();
}
