using API.Filters;
using API.Shared.Models;

namespace API.EndPoints.Demo;

public class HelloEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/demo/hello",
            async (HelloRequest request, CancellationToken cancellationToken) =>
            {
                await Task.Delay(100, cancellationToken);
                
                return Results.Ok(EndPointResponse<HelloResponse>.Success(
                    new HelloResponse { Message = $"Hello, {request.Name}!" }
                ));
            })
        .WithTags("Demo")
        .AddEndpointFilter<JwtEndpointFilter>()
        .Produces<EndPointResponse<HelloResponse>>();
    }
}

public class HelloRequest
{
    public string Name { get; set; } = string.Empty;
}

public class HelloResponse
{
    public string Message { get; set; } = string.Empty;
}

