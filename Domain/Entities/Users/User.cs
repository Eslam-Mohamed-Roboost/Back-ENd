using System.ComponentModel.DataAnnotations.Schema;
using API.Domain.Entities.General;
using API.Domain.Entities.Identity;
using API.Domain.Entities.Users;
using API.Domain.Enums;
 
namespace API.Domain.Entities;
[Table("User", Schema = "Identity")]
public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string SaltPassword { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public ApplicationRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    
    public List<Badges>? Badges { get; set; }
    
    public long? ClassID { get; set; }
    
    [ForeignKey(nameof(ClassID))]
    public Classes?  Classes { get; set; }
    
    public List<Token>?  Tokens { get; set; }
    
    
    public DateTime? LastLogin { get; set; }
}

