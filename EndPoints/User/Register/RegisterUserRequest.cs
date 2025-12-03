using API.Domain.Enums;

namespace API.EndPoints.User.Register;

public class RegisterUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public ApplicationRole RoleID { get; set; }
}

