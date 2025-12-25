using API.Application.Features.User.Export.Queries;
using API.Filters;
using API.Helpers;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.User.Export;

public class UserExportEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/User/Export",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new ExportUserDataQuery(), cancellationToken);
                    
                    if (result.IsSuccess)
                    {
                        using var workbook = ExcelHelper.GetExcel(result.Data!, "UserExportReport");
                        await using var memoryStream = new MemoryStream();
                        workbook.SaveAs(memoryStream);
                        var fileName = $"UserExportReport_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";

                        return Results.File(
                            memoryStream.ToArray(),
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileName
                        );
                    }
                    
                    return Response(RequestResult<bool>.Failure());
                })
            .WithTags("User")
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
