using API.Application.Features.Admin.Classes.Commands;
using API.Application.Features.Admin.Classes.DTOs;
using API.Application.Features.Admin.Classes.Queries;
using API.Filters;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.EndPoints.Admin.Classes;

public class ClassesEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        // GET /Admin/Classes - List all classes
        app.MapGet("/Admin/Classes",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetClassesQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<ClassDto>>>();

        // GET /Admin/Classes/Dropdown - Get classes for dropdown list (simplified)
        app.MapGet("/Admin/Classes/Dropdown",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetClassesDropdownQuery(), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<List<ClassDropdownDto>>>();

        // GET /Admin/Classes/{id} - Get class by ID
        app.MapGet("/Admin/Classes/{id}",
                async (long id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetClassByIdQuery(id), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<ClassDto>>();

        // POST /Admin/Classes - Create new class
        app.MapPost("/Admin/Classes",
                async ([FromBody] CreateClassRequest request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new CreateClassCommand(
                        request.Name,
                        request.Grade,
                        request.TeacherId);
                    
                    var result = await mediator.Send(command, cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<long>>();

        // PUT /Admin/Classes/{id} - Update class
        app.MapPut("/Admin/Classes/{id}",
                async (long id, [FromBody] UpdateClassRequest request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new UpdateClassCommand(
                        id,
                        request.Name,
                        request.Grade,
                        request.TeacherId);
                    
                    var result = await mediator.Send(command, cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();

        // DELETE /Admin/Classes/{id} - Delete class
        app.MapDelete("/Admin/Classes/{id}",
                async (long id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new DeleteClassCommand(id), cancellationToken);
                    return Response(result);
                })
            .WithTags("Admin")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Produces<EndPointResponse<bool>>();
    }
}

