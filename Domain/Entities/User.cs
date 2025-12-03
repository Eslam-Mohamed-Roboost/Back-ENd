using API.Domain.Enums;

namespace API.Domain.Entities;

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
}

