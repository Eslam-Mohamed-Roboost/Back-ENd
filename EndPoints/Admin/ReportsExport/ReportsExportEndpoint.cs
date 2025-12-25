using API.Application.Features.Admin.Reports.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.ReportsExport;

public class ReportsExportEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/Reports/Export",
                async (IMediator mediator, string reportType, string format, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new ExportReportQuery(reportType, format), cancellationToken);
                    
                    if (result.IsSuccess)
                    {
                        var (data, contentType, fileName) = result.Data!;
                        return Results.File(data, contentType, fileName);
                    }
                    
                    return Response(RequestResult<bool>.Failure());
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
