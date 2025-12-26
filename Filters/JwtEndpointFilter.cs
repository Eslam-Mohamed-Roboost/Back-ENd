using System.IdentityModel.Tokens.Jwt;
using System.Text;
using API.Domain.Entities.Redis;
using API.Infrastructure.Cache;
using API.Shared.Models;
using Dapper;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace API.Filters;

public class JwtEndpointFilter : IEndpointFilter
{
    private readonly ILogger<JwtEndpointFilter> _logger;
    private readonly IConfiguration _configuration;
    private readonly UserState _userState;
    private readonly IHybridCacheRepository _hybridCacheRepository;

    public JwtEndpointFilter(
        ILogger<JwtEndpointFilter> logger,
        IConfiguration configuration,
        UserState userState,
        IHybridCacheRepository hybridCacheRepository)
    {
        _logger = logger;
        _configuration = configuration;
        _userState = userState;
        _hybridCacheRepository = hybridCacheRepository;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        try
        {
            var httpContext = context.HttpContext;

            // Check if context is already disposed
            if (httpContext == null || httpContext.RequestAborted.IsCancellationRequested)
            {
                _logger.LogWarning("HttpContext is null or request was aborted");
                return Results.Unauthorized();
            }

            if (!httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                _logger.LogWarning("Authorization header is missing");
                return Results.Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.ToString().StartsWith("Bearer "))
            {
                _logger.LogWarning("Invalid Authorization header format");
                return Results.Unauthorized();
            }

            string token = authHeader.ToString().Substring("Bearer ".Length).Trim();

            var jwtId = await ValidateAndDecodeToken(token);
            if (jwtId == null)
            {
                _logger.LogWarning("Invalid JWT token");
                return Results.Unauthorized();
            }

            // Check token in cache first, then database
            bool isValid = await VerifyToken(jwtId.Value);
            if (!isValid)
            {
                _logger.LogWarning("Token {JwtId} not found in database or is invalid", jwtId);
                return Results.Unauthorized();
            }

            // Call next filter/endpoint and ensure result is awaited properly
            var result = await next(context);
            
            // Ensure we return the result without accessing HttpContext after disposal
            return result;
        }
        catch (OperationCanceledException)
        {
            // Request was cancelled - don't log as error, just return unauthorized
            _logger.LogWarning("Request was cancelled");
            return Results.Unauthorized();
        }
        catch (ObjectDisposedException ex)
        {
            // HttpContext was disposed - this can happen if request was aborted
            _logger.LogWarning(ex, "HttpContext was disposed during JWT filter execution");
            return Results.Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in JWT endpoint filter");
            // Don't throw - return unauthorized instead to prevent further exceptions
            return Results.Unauthorized();
        }
    }

    private async Task<bool> VerifyToken(long jwtId)
    {
        // Check cache first
        var cachedToken = await _hybridCacheRepository.GetAsync<CachedToken>(jwtId.ToString());
        
        if (cachedToken != null)
        {
            if (!cachedToken.IsActive || cachedToken.LoggedOutAt != null)
            {
                return false;
            }
            return true;
        }

        // Not in cache, check database
        var isValid = await VerifyTokenInDatabase(jwtId);

        // if (isValid)
        // {
        //     await SetAuthorizedTokenToCache(jwtId);
        // }
        // else
        // {
        //     await SetUnAuthorizedTokenInCache(jwtId);
        // }

        return isValid;
    }

    private async Task SetAuthorizedTokenToCache(long jwtId)
    {
        var token = new CachedToken
        {
            ID = jwtId,
            IsActive = true,
            ExpireAt = DateTime.UtcNow.AddDays(1),
            LoggedOutAt = null
        };

        await _hybridCacheRepository.SetAsync(jwtId.ToString(), token, TimeSpan.FromMinutes(30));
    }

    private async Task SetUnAuthorizedTokenInCache(long jwtId)
    {
        var token = new CachedToken
        {
            ID = jwtId,
            IsActive = false,
            ExpireAt = DateTime.UtcNow,
            LoggedOutAt = DateTime.UtcNow
        };

        await _hybridCacheRepository.SetAsync(jwtId.ToString(), token, TimeSpan.FromMinutes(5));
    }

    private async Task<long?> ValidateAndDecodeToken(string token)
    {
        try
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            var jtiClaim = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);
            
            if (jtiClaim != null && long.TryParse(jtiClaim.Value, out var jwtId))
            {
                return jwtId;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating JWT token");
            return null;
        }
    }

    private async Task<bool> VerifyTokenInDatabase(long jwtId)
    {
        try
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var user = await connection.QueryFirstOrDefaultAsync<dynamic>(
                @"SELECT u.""ID"", u.""Name"", u.""CompanyID"", u.""Role"" 
                  FROM ""Identity"".""User"" u 
                  INNER JOIN ""Identity"".""Token"" t ON u.""ID"" = t.""UserID""  
                  WHERE t.""JwtID"" = @JwtID 
                    AND t.""IsActive"" = TRUE 
                    AND (t.""LoggedOutAt"" IS NULL OR t.""LoggedOutAt"" > @Now)",
                new { JwtID = jwtId, Now = DateTime.UtcNow }
            );

            if (user == null)
                return false;

            // Set user state for the request
            _userState.SetUserData(
                (long)user.ID, 
                (Domain.Enums.ApplicationRole)user.Role, 
                (long)user.CompanyID, 
                (string)user.Name
            );

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying token in database");
            return false;
        }
    }
}

