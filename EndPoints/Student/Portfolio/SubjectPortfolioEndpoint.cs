using API.Application.Features.Student.Portfolio.DTOs;
using API.Application.Features.Student.Portfolio.Queries;
using API.Filters;
using API.Helpers.Attributes;
using API.Shared.Models;
using MediatR;
using System.Text.Json.Serialization;

namespace API.EndPoints.Student.Portfolio;

public class SubjectPortfolioEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Student/Portfolio/Subject/{subjectId}",
                async (IMediator mediator, [AsParameters] SubjectPortfolioRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetSubjectPortfolioQuery(request.SubjectId), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<SubjectPortfolioDto>>();
    }
}

public class SubjectPortfolioRequest
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }
}
