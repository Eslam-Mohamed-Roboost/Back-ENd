using API.Application.Features.Admin.Evidence.DTOs;
using API.Application.Features.Admin.Evidence.Queries;
using API.Filters;
using API.Helpers;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Admin.EvidenceExportCreate;

public class EvidenceExportCreateEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Admin/Evidence/Export",
                async (IMediator mediator, EvidenceExportRequestDto request, CancellationToken cancellationToken) =>
                {
                    var query = new ExportEvidenceDataQuery(
                        DateFrom: request.DateRange.Start,
                        DateTo: request.DateRange.End,
                        Subjects: request.Subjects,
                        EvidenceTypes: request.EvidenceTypes,
                        Format: request.Format
                    );
                    
                    var result = await mediator.Send(query, cancellationToken);
                    
                    if (result.IsSuccess)
                    {
                        using var workbook = ExcelHelper.GetExcel(result.Data!, "EvidenceExportReport");
                        await using var memoryStream = new MemoryStream();
                        workbook.SaveAs(memoryStream);
                        var fileName = $"EvidenceExportReport_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";

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
