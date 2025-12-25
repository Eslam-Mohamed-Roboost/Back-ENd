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

namespace API.EndPoints.Teacher.Portfolio;

public class TeacherPortfolioEndpoints : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        // GET /Teacher/Portfolio/MyStudents?subjectId=...&classId=...
        app.MapGet("/Teacher/Portfolio/MyStudents",
                async (IMediator mediator, long? subjectId, long? classId, CancellationToken cancellationToken) =>
                {
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

        // GET /Teacher/Portfolio/{studentId}/{subjectId}
        app.MapGet("/Teacher/Portfolio/{studentId}/{subjectId}",
                async (IMediator mediator, [AsParameters] PortfolioRouteParams routeParams, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetStudentPortfolioQuery(routeParams.StudentId, routeParams.SubjectId), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<TeacherPortfolioDto>>();

        // POST /Teacher/Portfolio/{studentId}/{subjectId}/Comment
        app.MapPost("/Teacher/Portfolio/{studentId}/{subjectId}/Comment",
                async (IMediator mediator, [AsParameters] PortfolioRouteParams routeParams, [FromBody] TeacherPortfolioCommentRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new AddPortfolioCommentCommand(routeParams.StudentId, routeParams.SubjectId, request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();

        // POST /Teacher/Portfolio/{studentId}/{subjectId}/ToggleLike
        app.MapPost("/Teacher/Portfolio/{studentId}/{subjectId}/ToggleLike",
                async (IMediator mediator, [AsParameters] PortfolioRouteParams routeParams, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new TogglePortfolioLikeCommand(routeParams.StudentId, routeParams.SubjectId), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();

        // POST /Teacher/Portfolio/{studentId}/{subjectId}/RequestRevision
        app.MapPost("/Teacher/Portfolio/{studentId}/{subjectId}/RequestRevision",
                async (IMediator mediator, [AsParameters] PortfolioRouteParams routeParams, [FromBody] TeacherPortfolioRevisionRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new RequestPortfolioRevisionCommand(routeParams.StudentId, routeParams.SubjectId, request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();

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

        // POST /Teacher/Portfolio/{studentId}/{subjectId}/AwardBadge
        app.MapPost("/Teacher/Portfolio/{studentId}/{subjectId}/AwardBadge",
                async (IMediator mediator, [AsParameters] PortfolioRouteParams routeParams, [FromBody] TeacherAwardPortfolioBadgeRequest request, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new AwardPortfolioBadgeCommand(routeParams.StudentId, routeParams.SubjectId, request), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<PortfolioBadgeDto>>();
    }
}

public class PortfolioRouteParams
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long StudentId { get; set; }

    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }
}


