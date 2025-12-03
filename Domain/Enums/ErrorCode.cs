using API.Helpers;

namespace API.Domain.Enums;

public enum ErrorCode
{
    [DescriptionAnnotation("none", "none")]
    None = 0,
    
    [DescriptionAnnotation("Invalid username or password", "Invalid username or password")]
    InvalidUserNameOrPassword = 1,
    
    [DescriptionAnnotation("Username or password cannot be empty", "Username or password cannot be empty")]
    EmptyUserNameOrPassword = 2,
    
    [DescriptionAnnotation("Username already exists", "Username already exists")]
    UserNameAlreadyExists = 3,
    
    [DescriptionAnnotation("Email already exists", "Email already exists")]
    EmailAlreadyExists = 4,
    
    [DescriptionAnnotation("User not found", "User not found")]
    UserNotFound = 5
}

