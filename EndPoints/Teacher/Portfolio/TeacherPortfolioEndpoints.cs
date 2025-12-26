using API.Application.Features.Teacher.Portfolio.DTOs;
using API.Application.Features.Teacher.Portfolio.Queries;
using API.Application.Features.Teacher.Portfolio.Commands;
using API.Application.Features.Student.Badges.DTOs;
using API.Filters;
using API.Helpers.Attributes;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using API.Application.Features.Student.Portfolio.DTOs;
using API.Application.Features.Teacher.Classes.DTOs;

namespace API.EndPoints.Teacher.Portfolio;

public class TeacherPortfolioEndpoints : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        // GET /Teacher/Portfolio/MyStudents?subjectId=...&classId=...
        app.MapGet("/Teacher/Portfolio/MyStudents",
                async (IMediator mediator, string? subjectIdStr, string? classIdStr, CancellationToken cancellationToken) =>
                {
                    // Parse subjectId - treat "null" string as null
                    long? subjectId = null;
                    if (!string.IsNullOrWhiteSpace(subjectIdStr) && subjectIdStr != "null" && long.TryParse(subjectIdStr, out long parsedSubjectId))
                    {
                        subjectId = parsedSubjectId;
                    }

                    // Parse classId - treat "null" string as null
                    long? classId = null;
                    if (!string.IsNullOrWhiteSpace(classIdStr) && classIdStr != "null" && long.TryParse(classIdStr, out long parsedClassId))
                    {
                        classId = parsedClassId;
                    }

                    var result = await mediator.Send(new GetMyStudentsQuery(subjectId, classId), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<StudentPortfolioDto>>>();

        // GET /Teacher/Portfolio/Students?subjectId=...
        app.MapGet("/Teacher/Portfolio/Students",
                async (IMediator mediator, long subjectId, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetSubjectStudentsQuery(subjectId), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<TeacherStudentSummaryDto>>>();

        // GET /Teacher/Portfolio/Student/{studentId}/{subjectId}
        app.MapGet("/Teacher/Portfolio/Student/{studentId}/{subjectId}",
                async (IMediator mediator, [FromRoute] string studentId, [FromRoute] string subjectId, CancellationToken cancellationToken) =>
                {
                    // Validate and parse route parameters
                    if (string.IsNullOrWhiteSpace(studentId) || studentId == "null" || 
                        string.IsNullOrWhiteSpace(subjectId) || subjectId == "null")
                    {
                        return Results.BadRequest(new { Message = "StudentId and SubjectId are required and cannot be null." });
                    }

                    if (!long.TryParse(studentId, out long studentIdLong) || !long.TryParse(subjectId, out long subjectIdLong))
                    {
                        return Results.BadRequest(new { Message = "Invalid StudentId or SubjectId format." });
                    }

                    var result = await mediator.Send(new GetStudentPortfolioQuery(studentIdLong, subjectIdLong), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<TeacherPortfolioDto>>();

        // POST /Teacher/Portfolio/Student/{studentId}/{subjectId}/Comment
        app.MapPost("/Teacher/Portfolio/Student/{studentId}/{subjectId}/Comment",
                async (IMediator mediator, [FromRoute] string studentId, [FromRoute] string subjectId, [FromBody] TeacherPortfolioCommentRequest request, CancellationToken cancellationToken) =>
                {
                    if (!long.TryParse(studentId, out long studentIdLong) || !long.TryParse(subjectId, out long subjectIdLong))
                    {
                        return Results.BadRequest(new { Message = "Invalid StudentId or SubjectId format." });
                    }

                    var result = await mediator.Send(new AddPortfolioCommentCommand(studentIdLong, subjectIdLong, request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();

        // POST /Teacher/Portfolio/Student/{studentId}/{subjectId}/ToggleLike
        app.MapPost("/Teacher/Portfolio/Student/{studentId}/{subjectId}/ToggleLike",
                async (IMediator mediator, [FromRoute] string studentId, [FromRoute] string subjectId, CancellationToken cancellationToken) =>
                {
                    if (!long.TryParse(studentId, out long studentIdLong) || !long.TryParse(subjectId, out long subjectIdLong))
                    {
                        return Results.BadRequest(new { Message = "Invalid StudentId or SubjectId format." });
                    }

                    var result = await mediator.Send(new TogglePortfolioLikeCommand(studentIdLong, subjectIdLong), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();

        // POST /Teacher/Portfolio/Student/{studentId}/{subjectId}/RequestRevision
        app.MapPost("/Teacher/Portfolio/Student/{studentId}/{subjectId}/RequestRevision",
                async (IMediator mediator, [FromRoute] string studentId, [FromRoute] string subjectId, [FromBody] TeacherPortfolioRevisionRequest request, CancellationToken cancellationToken) =>
                {
                    if (!long.TryParse(studentId, out long studentIdLong) || !long.TryParse(subjectId, out long subjectIdLong))
                    {
                        return Results.BadRequest(new { Message = "Invalid StudentId or SubjectId format." });
                    }

                    var result = await mediator.Send(new RequestPortfolioRevisionCommand(studentIdLong, subjectIdLong, request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();

        // GET /Teacher/Portfolio/MySubjects - Get teacher's assigned subjects
        app.MapGet("/Teacher/Portfolio/MySubjects",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetMySubjectsQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<ClassSubjectInfo>>>();

        // GET /Teacher/Portfolio/Badges
        app.MapGet("/Teacher/Portfolio/Badges",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetTeacherPortfolioBadgesQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<BadgeDto>>>();

        // POST /Teacher/Portfolio/Student/{studentId}/{subjectId}/AwardBadge
        app.MapPost("/Teacher/Portfolio/Student/{studentId}/{subjectId}/AwardBadge",
                async (IMediator mediator, [FromRoute] string studentId, [FromRoute] string subjectId, [FromBody] TeacherAwardPortfolioBadgeRequest request, CancellationToken cancellationToken) =>
                {
                    if (!long.TryParse(studentId, out long studentIdLong) || !long.TryParse(subjectId, out long subjectIdLong))
                    {
                        return Results.BadRequest(new { Message = "Invalid StudentId or SubjectId format." });
                    }

                    var result = await mediator.Send(new AwardPortfolioBadgeCommand(studentIdLong, subjectIdLong, request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<PortfolioBadgeDto>>();
    }
}


