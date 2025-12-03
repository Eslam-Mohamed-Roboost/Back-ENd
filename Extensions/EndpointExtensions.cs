using Scalar.AspNetCore;

namespace API.Extensions;

public static class EndpointExtensions
{
    public static void ConfigureEndpoints(this WebApplication app)
    {
        // Redirect root to Scalar documentation
        app.MapGet("/", () => Results.Redirect("/scalar/v1"))
            .ExcludeFromDescription();
        
        // Map OpenAPI specification
        app.MapOpenApi();
        
        // Map Scalar API Reference UI
        app.MapScalarApiReference();
        
        // Map MVC controllers (if any)
        app.MapControllers();
    }
}

