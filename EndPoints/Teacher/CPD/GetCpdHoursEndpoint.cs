using API.Application.Features.Teacher.CPD.Queries;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.EndPoints.Teacher.CPD;

[Authorize(Roles = "Teacher")]
public class GetCpdHoursEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Teacher/CPD/Hours", HandleAsync)
            .WithName("GetCpdHours")
            .WithTags("Teacher CPD")
            .WithOpenApi();
    }

    private async Task<IResult> HandleAsync(
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetCpdHoursSummaryQuery();
        var result = await mediator.Send(query, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}

