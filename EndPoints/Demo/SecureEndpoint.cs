using API.Application.Features.Demo.Query;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Demo;

public class SecureEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/demo/secure",
            async (UserState userState,IMediator _mediator, CancellationToken cancellationToken) =>
            {
                await Task.Delay(100, cancellationToken);
                var result = await _mediator.Send(new UserStateQuery(), cancellationToken);
              return Response(result);
            })
        .WithTags("Demo")
        .AddEndpointFilter<JwtEndpointFilter>()
        .Produces<EndPointResponse<string>>();
    }
}

public class SecureResponse
{
    public string Message { get; set; } = string.Empty;
    public long UserID { get; set; }
    public string Role { get; set; } = string.Empty;
}

