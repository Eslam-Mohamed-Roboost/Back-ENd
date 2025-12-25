using API.Application.Features.Student.Portfolio.Commands;
using API.Filters;
using API.Helpers.Attributes;
using API.Shared.Models;
using MediatR;
using System.Text.Json.Serialization;

namespace API.EndPoints.Student.Portfolio;

public class PortfolioFileDeleteEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/Student/Portfolio/File/{fileId}",
                async (IMediator mediator, [AsParameters] DeleteFileRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new DeletePortfolioFileCommand(request.FileId), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();
    }
}

public class DeleteFileRequest
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long FileId { get; set; }
}
