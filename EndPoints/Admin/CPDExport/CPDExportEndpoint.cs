using API.Application.Features.Admin.CPD.Queries;
using API.Filters;
using API.Helpers;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.CPDExport;

public class CPDExportEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/CPD/Export",
                async (IMediator mediator, string? format, DateTime? dateFrom, DateTime? dateTo, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new ExportCPDDataQuery(format, dateFrom, dateTo), cancellationToken);
                    
                    if (result.IsSuccess)
                    {
                        using var workbook = ExcelHelper.GetExcel(result.Data!, "CPDExportReport");
                        await using var memoryStream = new MemoryStream();
                        workbook.SaveAs(memoryStream);
                        var fileName = $"CPDExportReport_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";

                        return Results.File(
                            memoryStream.ToArray(),
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileName
                        );
                    }
                    
                    return Response(RequestResult<bool>.Failure());
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
