using API.Application.Features.Student.PortfolioBook.Commands;
using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Filters;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.EndPoints.Student.PortfolioBook;

public class SavePortfolioProjectEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Student/PortfolioBook/Project",
                async (IMediator mediator, HttpRequest httpRequest, CancellationToken cancellationToken) =>
                {
                    var form = await httpRequest.ReadFormAsync(cancellationToken);
                    
                    var request = new SavePortfolioProjectRequest
                    {
                        SubjectId = long.Parse(form["SubjectId"].ToString()),
                        Title = form["Title"].ToString(),
                        Type = form["Type"].ToString(),
                        Description = form["Description"].ToString(),
                        SkillsUsed = form["SkillsUsed"].ToString(),
                        WhatLearned = form["WhatLearned"].ToString()
                    };

                    // TODO: Handle file uploads and store them
                    var fileUrls = new List<string>();
                    foreach (var file in form.Files)
                    {
                        // Placeholder: In production, save to storage and get URL
                        fileUrls.Add($"/uploads/{file.FileName}");
                    }

                    var result = await mediator.Send(new SavePortfolioProjectCommand(request, fileUrls), cancellationToken);
                    return Response(result);
                })
            .WithTags("Student")
            .AddEndpointFilter<JwtEndpointFilter>()
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<EndPointResponse<PortfolioProjectDto>>();
    }
}
