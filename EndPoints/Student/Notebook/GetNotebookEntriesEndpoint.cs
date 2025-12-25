using API.Application.Features.Student.Notebook.DTOs;
using API.Application.Features.Student.Notebook.Queries;
using API.Filters;
using API.Helpers.Attributes;
using API.Shared.Models;
using MediatR;
using System.Text.Json.Serialization;

namespace API.EndPoints.Student.Notebook;

public class GetNotebookEntriesEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Student/Notebook",
                async (IMediator mediator, long? subjectId, DateTime? dateFrom, DateTime? dateTo, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetNotebookEntriesQuery(subjectId, dateFrom, dateTo), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<NotebookEntryDto>>>();
    }
}
