using API.Domain.Enums;

namespace API.EndPoints.User.GetUsers;

public class GetUserRequest : PageingRequest
{
    public string? email { get; set; }
    public ApplicationRole? role { get; set; }
    public bool? IsActve { get; set; }
    public long? classId { get; set; }
}