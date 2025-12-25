using API.Application.Features.Student.Notebook.Commands;
using API.Filters;
using API.Helpers.Attributes;
using API.Shared.Models;
using MediatR;
using System.Text.Json.Serialization;

namespace API.EndPoints.Student.Notebook;

public class DeleteNotebookEntryEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/Student/Notebook/{entryId}",
                async (IMediator mediator, [AsParameters] DeleteEntryRouteParams routeParams, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new DeleteNotebookEntryCommand(routeParams.EntryId), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();
    }
}

public class DeleteEntryRouteParams
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long EntryId { get; set; }
}
