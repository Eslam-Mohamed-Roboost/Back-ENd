using API.Domain.Enums;

namespace API.EndPoints.User.Login;

public class UserLoginResponse
{
    public long ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ApplicationRole Role { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Authorization { get; set; } = string.Empty;
}

