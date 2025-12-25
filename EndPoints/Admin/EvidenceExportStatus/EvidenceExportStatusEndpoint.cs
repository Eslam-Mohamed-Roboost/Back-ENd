using API.Application.Features.Admin.Evidence.DTOs;
using API.Filters;
using API.Shared.Models;

namespace API.EndPoints.Admin.EvidenceExportStatus;

public class EvidenceExportStatusEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/Admin/Evidence/Export/{exportId}",
                (string exportId, CancellationToken cancellationToken) =>
                {
                    var result = new EvidenceExportStatusDto
                    {
                        ExportId = exportId,
                        Status = "completed",
                        DownloadUrl = $"https://storage.school.ae/exports/{exportId}.zip",
                        ExpiresAt = DateTime.UtcNow.AddDays(7)
                    };
                    var result1 =   RequestResult<dynamic>.Success(result);
                    return Response(result1);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
