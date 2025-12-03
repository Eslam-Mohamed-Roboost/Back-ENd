using API.Domain.Enums;
using API.Shared.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Middlewares;

public class UserStateInitializerMiddleware : IMiddleware
{
    private readonly UserState _userState;

    public UserStateInitializerMiddleware(UserState userState)
    {
        _userState = userState;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        
        if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
            
            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);
                
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                
                if (long.TryParse(userIdClaim, out var userId) && Enum.TryParse<ApplicationRole>(roleClaim, out var role))
                {
                    _userState.SetUserData(userId, role, 0, "User");
                }
            }
        }

        await next(context);
    }
}

