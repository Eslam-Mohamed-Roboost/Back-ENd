using API.Application.Features.Teacher.Lounge.DTOs;
using API.Application.Features.Teacher.Lounge.Queries;
using API.Application.Features.Admin.Announcements.DTOs;
using API.Filters;
using API.Shared.Models;
using MediatR;

namespace API.EndPoints.Teacher.Lounge;

public class TeachersLoungeEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        // GET /Teacher/Lounge - Get teachers lounge data (leaderboards and stats)
        app.MapGet("/Teacher/Lounge",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetTeachersLoungeQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<TeachersLoungeDto>>();

        // GET /Teacher/Lounge/Announcements - Get announcements for teachers
        app.MapGet("/Teacher/Lounge/Announcements",
                async (IMediator mediator, int limit, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetTeacherAnnouncementsQuery(limit), cancellationToken);
                    return Response(result);
                })
            .WithTags("Teacher")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<AnnouncementDto>>>();
    }
}

