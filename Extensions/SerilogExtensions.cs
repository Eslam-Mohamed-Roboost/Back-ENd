using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog.Events;

namespace API.Extensions;

public static class SerilogExtensions
{
    public static void ConfigureSerilog(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                theme: AnsiConsoleTheme.Code,
                restrictedToMinimumLevel: LogEventLevel.Information)
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(evt => 
                    evt.Properties.ContainsKey("CommandText") || 
                    evt.MessageTemplate.Text.Contains("Executing") ||
                    evt.MessageTemplate.Text.Contains("Executed"))
                .WriteTo.Console(
                    outputTemplate: "\u001b[36m[SQL Query]\u001b[0m {Message:lj}{NewLine}\u001b[33m{Properties:j}\u001b[0m{NewLine}",
                    theme: AnsiConsoleTheme.Literate))
            .CreateLogger();

        builder.Host.UseSerilog();
    }
}

