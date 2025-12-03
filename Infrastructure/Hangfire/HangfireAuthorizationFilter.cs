using Hangfire.Dashboard;
using System.Text;

namespace API.Infrastructure.Hangfire;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly string _username;
    private readonly string _password;

    public HangfireAuthorizationFilter(string username, string password)
    {
        _username = username;
        _password = password;
    }

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Allow access in development without authentication
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            return true;
        }

        // Basic Authentication
        var authHeader = httpContext.Request.Headers["Authorization"].ToString();
        
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic "))
        {
            httpContext.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Hangfire Dashboard\"";
            httpContext.Response.StatusCode = 401;
            return false;
        }

        try
        {
            var encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
            var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
            var credentials = decodedCredentials.Split(':', 2);

            if (credentials.Length == 2 && 
                credentials[0] == _username && 
                credentials[1] == _password)
            {
                return true;
            }
        }
        catch
        {
            // Invalid credentials format
        }

        httpContext.Response.StatusCode = 403;
        return false;
    }
}

