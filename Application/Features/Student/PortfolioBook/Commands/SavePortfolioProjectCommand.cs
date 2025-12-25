using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Student.PortfolioBook.Commands;

public record SavePortfolioProjectCommand(SavePortfolioProjectRequest Request, List<string> FileUrls) : IRequest<RequestResult<PortfolioProjectDto>>;

public class SavePortfolioProjectCommandHandler(
    RequestHandlerBaseParameters parameters)
    : RequestHandlerBase<SavePortfolioProjectCommand, RequestResult<PortfolioProjectDto>>(parameters)
{
    public override async Task<RequestResult<PortfolioProjectDto>> Handle(SavePortfolioProjectCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // TODO: Implement database save and file upload when entity is created
        var project = new PortfolioProjectDto
        {
            Id = 1, // Generate ID on create
            Title = request.Request.Title,
            Type = request.Request.Type,
            Description = request.Request.Description,
            SkillsUsed = request.Request.SkillsUsed,
            WhatLearned = request.Request.WhatLearned,
            FileUrls = request.FileUrls,
            CreatedDate = DateTime.UtcNow
        };

        return RequestResult<PortfolioProjectDto>.Success(project);
    }
}
