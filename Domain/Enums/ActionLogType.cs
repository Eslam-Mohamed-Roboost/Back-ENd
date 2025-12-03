namespace API.Domain.Enums;

public enum ActionLogType
{
    UserLogin = 1,
    UserLogout = 2,
    UserRegistration = 3,
    DataCreated = 4,
    DataUpdated = 5,
    DataDeleted = 6,
    PasswordChanged = 7,
    PermissionChanged = 8,
    ConfigurationChanged = 9,
    IncentiveLog = 10,
    OrderLog = 11
}

