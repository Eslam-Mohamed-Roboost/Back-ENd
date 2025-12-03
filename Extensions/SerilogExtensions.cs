using Serilog;

namespace API.Extensions;

public static class SerilogExtensions
{
    public static void ConfigureSerilog(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Host.UseSerilog();
    }
}

