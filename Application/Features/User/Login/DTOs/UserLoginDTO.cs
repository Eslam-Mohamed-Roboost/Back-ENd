using API.Domain.Enums;

namespace API.Application.Features.User.Login.DTOs;

public class UserLoginDTO
{
    public long ID { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string SaltPassword { get; set; } = string.Empty;
    public ApplicationRole Role { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Authorization { get; set; } = string.Empty;
}

