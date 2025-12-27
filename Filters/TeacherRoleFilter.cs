using API.Domain.Enums;
using API.Shared.Models;

namespace API.Filters;

public class TeacherRoleFilter : IEndpointFilter
{
    private readonly UserState _userState;

    public TeacherRoleFilter(UserState userState)
    {
        _userState = userState;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (_userState.RoleID != ApplicationRole.Teacher)
        {
            return Results.Json(
                EndPointResponse<object>.Failure(ErrorCode.Unauthorized, "Access denied. Teacher role required.", false),
                statusCode: 403);
        }

        return await next(context);
    }
}
