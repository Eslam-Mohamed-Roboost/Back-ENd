using API.Application.Features.Teacher.Attendance.DTOs;
using API.Application.Features.Teacher.Attendance.Queries;
using API.Application.Features.Teacher.Attendance.Commands;
using API.Filters;
using API.Helpers.Attributes;
using API.Shared.Models;
using MediatR;
using System.Text.Json.Serialization;

namespace API.EndPoints.Teacher.Attendance;

public class TeacherAttendanceEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        // GET /Teacher/Classes/{classId}/Attendance/{date} - Get class attendance for date
        app.MapGet("/Teacher/Classes/{classId}/Attendance/{date}",
                async (IMediator mediator, [AsParameters] ClassAttendanceRouteParams routeParams, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetClassAttendanceQuery(routeParams.ClassId, routeParams.Date), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<ClassAttendanceDto>>();

        // POST /Teacher/Classes/{classId}/Attendance - Mark attendance (manual)
        app.MapPost("/Teacher/Classes/{classId}/Attendance",
                async (IMediator mediator, [AsParameters] ClassAttendancePostRouteParams routeParams, MarkAttendanceRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new MarkAttendanceCommand(routeParams.ClassId, request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();

        // POST /Teacher/Classes/{classId}/Attendance/Bulk - Bulk mark attendance
        app.MapPost("/Teacher/Classes/{classId}/Attendance/Bulk",
                async (IMediator mediator, [AsParameters] ClassAttendancePostRouteParams routeParams, BulkMarkAttendanceRequest request, CancellationToken cancellationToken) =>
                {
                    // Ensure ClassId matches
                    request.ClassId = routeParams.ClassId;
                    var result = await mediator.Send(new BulkMarkAttendanceCommand(request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();

        // PUT /Teacher/Attendance/{attendanceId} - Update attendance record
        app.MapPut("/Teacher/Attendance/{attendanceId}",
                async (IMediator mediator, [AsParameters] UpdateAttendanceRouteParams routeParams, UpdateAttendanceRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new UpdateAttendanceCommand(routeParams.AttendanceId, request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();

        // POST /Teacher/Classes/{classId}/Attendance/ProcessAutomatic - Process automatic attendance
        app.MapPost("/Teacher/Classes/{classId}/Attendance/ProcessAutomatic",
                async (IMediator mediator, [AsParameters] ProcessAutomaticAttendanceRouteParams routeParams, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new ProcessAutomaticAttendanceCommand(routeParams.Date, routeParams.ClassId), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<int>>();
    }
}

public class ClassAttendanceRouteParams
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long ClassId { get; set; }
    public DateTime Date { get; set; }
}

public class ClassAttendancePostRouteParams
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long ClassId { get; set; }
}

public class UpdateAttendanceRouteParams
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long AttendanceId { get; set; }
}

public class ProcessAutomaticAttendanceRouteParams
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long ClassId { get; set; }
    public DateTime Date { get; set; }
}

