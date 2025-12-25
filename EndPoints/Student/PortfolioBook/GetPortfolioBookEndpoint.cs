using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Application.Features.Student.PortfolioBook.Queries;
using API.Filters;
using API.Helpers.Attributes;
using API.Shared.Models;
using MediatR;
using System.Text.Json.Serialization;

namespace API.EndPoints.Student.PortfolioBook;

public class GetPortfolioBookEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Student/PortfolioBook/{subjectId}",
                async (IMediator mediator, [AsParameters] PortfolioBookRouteParams routeParams, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetPortfolioBookQuery(routeParams.SubjectId), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<PortfolioBookDto>>();
    }
}

public class PortfolioBookRouteParams
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }
}
