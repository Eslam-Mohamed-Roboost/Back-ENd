using API.Application.Features.Admin.Reports.DTOs;
using API.Application.Features.Admin.Reports.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.ReportsGenerate;

public class ReportsGenerateEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/Reports/Generate",
                async (IMediator mediator, string reportType, DateTime? dateFrom, DateTime? dateTo, string? userType, bool? includeCharts, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GenerateReportQuery(reportType, dateFrom, dateTo, userType, includeCharts ?? false), cancellationToken);
                    return Response(result);
                 })
            .WithTags("Admin")
            .Produces<EndPointResponse<ReportDto>>()
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
