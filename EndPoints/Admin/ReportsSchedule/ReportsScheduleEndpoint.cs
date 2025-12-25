using API.Application.Features.Admin.Reports.DTOs;
using API.Filters;
using API.Shared.Models;

namespace API.EndPoints.Admin.ReportsSchedule;

public class ReportsScheduleEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Admin/Reports/Schedule",
                async (ScheduleReportDto request, CancellationToken cancellationToken) =>
                {

                    var result = new
                    {
                        success = true,
                        message = "Report scheduled successfully",
                        scheduleId = Guid.NewGuid().ToString()
                    };
                    var result1 =   RequestResult<dynamic>.Success(result);
                    return Response(result1);
                 })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>();
    }
}
