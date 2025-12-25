using API.Application.Features.Admin.Portfolio.DTOs;
using API.Application.Features.Admin.Portfolio.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.PortfolioStatsBySubject;

public class PortfolioStatsBySubjectEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/Portfolio/StatsBySubject",
                async (IMediator mediator, string? subjectName, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetPortfolioStatsBySubjectQuery(subjectName), cancellationToken);
                    return Response(result);
                   
                })
            .WithTags("Admin")
            .Produces<EndPointResponse<List<SubjectPortfolioStatsDto>>>()
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
