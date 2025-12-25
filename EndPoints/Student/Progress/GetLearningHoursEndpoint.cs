using API.Application.Features.Student.Progress.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.EndPoints.Student.Progress;

public class GetLearningHoursEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Student/Progress/LearningHours", HandleAsync)
            .WithName("GetLearningHours")
            .WithTags("Student Progress")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<API.Application.Features.Student.Progress.DTOs.LearningHoursSummaryDto>>()
            .WithOpenApi();
    }

    private async Task<IResult> HandleAsync(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetLearningHoursQuery(startDate, endDate);
        var result = await mediator.Send(query, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}

