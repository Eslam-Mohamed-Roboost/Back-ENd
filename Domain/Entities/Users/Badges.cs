using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.Users;
[Table("Badges", Schema = "User")]

public class Badges:BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
    [ForeignKey(nameof(User))]
    public long? UserId { get; set; }
    
    public User? User { get; set; }
}