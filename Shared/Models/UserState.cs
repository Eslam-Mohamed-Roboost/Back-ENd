using API.Domain.Enums;

namespace API.Shared.Models;

public class UserState
{
    public ApplicationRole RoleID { get; set; }
    public long UserID { get; set; }
    public long CompanyID { get; set; }
    public string Username { get; set; } = "System";
    
    public void SetUserData(long? userID, ApplicationRole roleID, long? companyID, string? userName)
    {
        UserID = userID ?? 0;
        RoleID = roleID;
        Username = userName ?? "System";
        CompanyID = companyID ?? 0;
    }
}

