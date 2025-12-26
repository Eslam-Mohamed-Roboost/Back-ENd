using Microsoft.Extensions.Logging;
using System.Text;

namespace API.Helpers;

public class SqlLogger : ILogger
{
    private readonly string _categoryName;

    public SqlLogger(string categoryName)
    {
        _categoryName = categoryName;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true; // Enable all log levels to catch exceptions

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        // Log all levels including errors and warnings
        if (logLevel < LogLevel.Information && exception == null) return;

        var message = formatter(state, exception);
        
        // Color codes for terminal
        const string Reset = "\u001b[0m";
        const string Cyan = "\u001b[36m";      // SQL queries
        const string Yellow = "\u001b[33m";     // Parameters
        const string Green = "\u001b[32m";      // Execution time
        const string Magenta = "\u001b[35m";    // Command type
        const string Blue = "\u001b[34m";       // Headers
        const string Red = "\u001b[31m";        // Errors
        const string Orange = "\u001b[38;5;208m"; // Warnings

        // Check if this is a database command log (executing, executed, or failed)
        if (message.Contains("Executing DbCommand") || 
            message.Contains("Executed DbCommand") || 
            message.Contains("Failed executing DbCommand") ||
            message.Contains("Failed executing") ||
            exception != null)
        {
            var lines = message.Split('\n');
            var output = new StringBuilder();
            
            // Determine if this is an error
            bool isError = exception != null || 
                          logLevel >= LogLevel.Error || 
                          message.Contains("Failed") ||
                          message.Contains("Exception") ||
                          message.Contains("Error");
            
            bool isWarning = logLevel == LogLevel.Warning;
            
            // Header color based on status
            string headerColor = isError ? Red : (isWarning ? Orange : Blue);
            string statusIcon = isError ? "✗" : (isWarning ? "⚠" : "▶");
            string statusText = isError ? "FAILED" : (isWarning ? "WARNING" : "Executing");
            
            output.AppendLine($"{headerColor}═══════════════════════════════════════════════════════════════{Reset}");
            
            // Log exception first if present
            if (exception != null)
            {
                output.AppendLine($"{Red}✗ EXCEPTION: {exception.GetType().Name}{Reset}");
                output.AppendLine($"{Red}Message: {exception.Message}{Reset}");
                if (!string.IsNullOrWhiteSpace(exception.StackTrace))
                {
                    output.AppendLine($"{Red}Stack Trace:{Reset}");
                    var stackLines = exception.StackTrace.Split('\n').Take(5);
                    foreach (var stackLine in stackLines)
                    {
                        output.AppendLine($"{Red}  {stackLine.Trim()}{Reset}");
                    }
                }
                output.AppendLine();
            }
            
            foreach (var line in lines)
            {
                if (line.Contains("Executing DbCommand") || line.Contains("Failed executing DbCommand"))
                {
                    output.AppendLine($"{headerColor}{statusIcon} {statusText} DbCommand{Reset}");
                }
                else if (line.Contains("Executed DbCommand"))
                {
                    var timeMatch = System.Text.RegularExpressions.Regex.Match(line, @"took (\d+\.?\d*)");
                    if (timeMatch.Success)
                    {
                        output.AppendLine($"{Green}✓ Executed in {timeMatch.Groups[1].Value}ms{Reset}");
                    }
                    else
                    {
                        output.AppendLine($"{Green}✓ Executed{Reset}");
                    }
                }
                else if (line.Trim().StartsWith("SELECT") || 
                         line.Trim().StartsWith("INSERT") || 
                         line.Trim().StartsWith("UPDATE") || 
                         line.Trim().StartsWith("DELETE") ||
                         line.Trim().StartsWith("WITH") ||
                         line.Trim().StartsWith("CREATE") ||
                         line.Trim().StartsWith("ALTER") ||
                         line.Trim().StartsWith("DROP"))
                {
                    // SQL Query - Cyan color (or Red if error)
                    string queryColor = isError ? Red : Cyan;
                    output.AppendLine($"{queryColor}{line.Trim()}{Reset}");
                }
                else if (line.Contains("Parameters:"))
                {
                    output.AppendLine($"{Yellow}Parameters:{Reset}");
                }
                else if (line.Contains("=") && line.Contains("@"))
                {
                    // Parameter - Yellow color
                    output.AppendLine($"{Yellow}  {line.Trim()}{Reset}");
                }
                else if (line.Contains("Exception") || line.Contains("Error") || line.Contains("Failed"))
                {
                    // Error messages - Red color
                    output.AppendLine($"{Red}{line.Trim()}{Reset}");
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    output.AppendLine(line);
                }
            }
            
            output.AppendLine($"{headerColor}═══════════════════════════════════════════════════════════════{Reset}");
            
            Console.WriteLine(output.ToString());
        }
        else if (message.Contains("Microsoft.EntityFrameworkCore"))
        {
            // Other EF Core messages - less prominent
            string color = logLevel >= LogLevel.Error ? Red : (logLevel == LogLevel.Warning ? Orange : Blue);
            Console.WriteLine($"{color}[EF Core]{Reset} {message}");
        }
    }
}

public class SqlLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        if (categoryName == "Microsoft.EntityFrameworkCore.Database.Command")
        {
            return new SqlLogger(categoryName);
        }
        // Return a simple logger for other categories
        return new SimpleLogger(categoryName);
    }

    public void Dispose() { }
}

internal class SimpleLogger : ILogger
{
    private readonly string _categoryName;

    public SimpleLogger(string categoryName)
    {
        _categoryName = categoryName;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    public bool IsEnabled(LogLevel logLevel) => false;
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
}

