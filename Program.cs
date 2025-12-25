using API.Domain.UnitOfWork;
using API.EndPoints;
using API.EndPoints.User.Login;
using API.Extensions;
using API.Middlewares;
using API.Shared.Models;
using FluentValidation;
using IdGen;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    Log.Information("Starting Read App Settings");
    builder.ReadAppSettings();
    Log.Information("End Read App Settings");

    builder.ConfigureSerilog();
    Log.Information("Starting APP");

    // Configure Services
    builder.Services.ConfigureServices(builder.Configuration);
    builder.Services.ConfigurePersistence(builder.Configuration, builder.Environment);
    builder.Services.ConfigureResponseCompression(builder.Configuration);
    builder.Services.ConfigureCors(builder.Configuration);
    builder.Services.ConfigureJwt(builder.Configuration);

    // Register Core Services
    builder.Services.AddScoped<UserState>();
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<RequestHandlerBaseParameters>();
    builder.Services.AddScoped<TransactionMiddleware>();
    builder.Services.AddScoped<UserStateInitializerMiddleware>();
    builder.Services.AddSingleton(new IdGenerator(0));
    
    // Register Application Services
    builder.Services.AddScoped<API.Application.Features.Teacher.Permissions.Services.TeacherPermissionService>();

    // Configure HybridCache/Redis
    builder.ConfigureHybridCache();

    // Configure Rate Limiting
    builder.ConfigureRateLimiting();

    // Register FluentValidation
    builder.Services.AddValidatorsFromAssemblyContaining<UserLoginRequest>();

    builder.Services.AddHttpContextAccessor();

    var app = builder.Build();

    // Initialize Database
    await app.Services.InitializeDatabaseAsync();

    app.EnableCors();
    app.UseAuthentication();
    app.UseAuthorization();

    // Global Exception Middleware should be early in pipeline, before other middleware that might write responses
    app.UseMiddleware<GlobalExceptionMiddleware>();

    // Auto-register endpoints
    var endpointDefinitions = typeof(Program).Assembly
        .GetTypes()
        .Where(t => typeof(EndpointDefinition).IsAssignableFrom(t) && !t.IsAbstract)
        .Select(Activator.CreateInstance)
        .Cast<EndpointDefinition>();

    foreach (var endpoint in endpointDefinitions)
    {
        endpoint.RegisterEndpoints(app);
    }

    // Use Rate Limiter
    app.UseRateLimiter();
    
    // Enable Response Compression
    app.EnableResponseCompression();

    // Other Middleware Pipeline
    app.UseMiddleware<RequestTimeoutMiddleware>();
    app.UseMiddleware<SlowRequestLoggingMiddleware>();
    app.UseMiddleware<UserStateInitializerMiddleware>();
    app.UseMiddleware<TransactionMiddleware>();

    app.UseSerilogRequestLogging();

    app.ConfigureEndpoints();

    app.Run();

    Log.Information("APP STARTED");
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application terminated unexpectedly");
    Console.WriteLine(exception.ToString());
}
finally
{
    Log.Error("Shut down complete");
    Log.CloseAndFlush();
}
