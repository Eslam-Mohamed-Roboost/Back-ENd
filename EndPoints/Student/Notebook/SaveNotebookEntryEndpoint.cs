using API.Application.Features.Student.Notebook.Commands;
using API.Application.Features.Student.Notebook.DTOs;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Student.Notebook;

public class SaveNotebookEntryEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Student/Notebook",
                async (IMediator mediator, SaveNotebookEntryRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new SaveNotebookEntryCommand(request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<NotebookEntryDto>>();
    }
}
