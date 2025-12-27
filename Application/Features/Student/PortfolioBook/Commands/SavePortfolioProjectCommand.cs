using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Domain.Entities.Portfolio;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Student.PortfolioBook.Commands;

public record SavePortfolioProjectCommand(SavePortfolioProjectRequest Request, List<string> FileUrls) : IRequest<RequestResult<PortfolioProjectDto>>;

public class SavePortfolioProjectCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<PortfolioBookProject> projectRepository)
    : RequestHandlerBase<SavePortfolioProjectCommand, RequestResult<PortfolioProjectDto>>(parameters)
{
    public override async Task<RequestResult<PortfolioProjectDto>> Handle(SavePortfolioProjectCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        var entity = new PortfolioBookProject
        {
            StudentId = studentId,
            SubjectId = request.Request.SubjectId,
            Title = request.Request.Title,
            Type = request.Request.Type,
            Description = request.Request.Description,
            SkillsUsed = request.Request.SkillsUsed,
            WhatLearned = request.Request.WhatLearned,
            FileUrls = request.FileUrls
        };

        projectRepository.Add(entity);
        await projectRepository.SaveChangesAsync(cancellationToken);

        var project = new PortfolioProjectDto
        {
            Id = entity.ID,
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
