using System.IO.Compression;
using Microsoft.AspNetCore.ResponseCompression;

namespace API.Extensions;

public static class ResponseCompressionExtensions
{
    public static void ConfigureResponseCompression(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<GzipCompressionProvider>();
            options.Providers.Add<BrotliCompressionProvider>();
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });

        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });
    }

    public static void EnableResponseCompression(this IApplicationBuilder app)
    {
        app.UseResponseCompression();
    }
}

