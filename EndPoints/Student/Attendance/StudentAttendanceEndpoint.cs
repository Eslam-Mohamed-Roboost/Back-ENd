using API.Application.Features.Student.Attendance.DTOs;
using API.Application.Features.Student.Attendance.Queries;
using API.Application.Features.Student.Attendance.Commands;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Student.Attendance;

public class StudentAttendanceEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        // GET /Student/Attendance - Get student's attendance history
        app.MapGet("/Student/Attendance",
                async (IMediator mediator, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetStudentAttendanceQuery(startDate, endDate), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<StudentAttendanceHistoryDto>>>();

        // GET /Student/Attendance/Statistics - Get attendance statistics
        app.MapGet("/Student/Attendance/Statistics",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetAttendanceStatisticsQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<AttendanceStatisticsDto>>();

        // POST /Student/Attendance/CalculateBonus - Calculate attendance bonus
        app.MapPost("/Student/Attendance/CalculateBonus",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new CalculateAttendanceBonusCommand(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<AttendanceBonusResult>>();

        // POST /Student/Attendance/AwardBonus - Award attendance bonus
        app.MapPost("/Student/Attendance/AwardBonus",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new AwardAttendanceBonusCommand(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<AttendanceBonusAwardResult>>();
    }
}

