using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Domain.Entities.Portfolio;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.PortfolioBook.Commands;

public record SavePortfolioLearningStyleCommand(SavePortfolioLearningStyleRequest Request) : IRequest<RequestResult<PortfolioLearningStyleDto>>;

public class SavePortfolioLearningStyleCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<PortfolioBookLearningStyle> learningStyleRepository)
    : RequestHandlerBase<SavePortfolioLearningStyleCommand, RequestResult<PortfolioLearningStyleDto>>(parameters)
{
    public override async Task<RequestResult<PortfolioLearningStyleDto>> Handle(SavePortfolioLearningStyleCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        var alreadySubmitted = await learningStyleRepository.Get(x => x.StudentId == studentId && x.SubjectId == request.Request.SubjectId)
            .AnyAsync(cancellationToken);

        if (alreadySubmitted)
        {
            return RequestResult<PortfolioLearningStyleDto>.Failure(ErrorCode.BadRequest, "You have already submitted your learning style.");
        }

        var entity = new PortfolioBookLearningStyle
        {
            StudentId = studentId,
            SubjectId = request.Request.SubjectId,
            LearnsBestBy = request.Request.LearnsBestBy,
            BestTimeToStudy = request.Request.BestTimeToStudy,
            FocusConditions = request.Request.FocusConditions,
            HelpfulTools = request.Request.HelpfulTools,
            Distractions = request.Request.Distractions
        };

        learningStyleRepository.Add(entity);
        await learningStyleRepository.SaveChangesAsync(cancellationToken);

        var learningStyle = new PortfolioLearningStyleDto
        {
            LearnsBestBy = request.Request.LearnsBestBy,
            BestTimeToStudy = request.Request.BestTimeToStudy,
            FocusConditions = request.Request.FocusConditions,
            HelpfulTools = request.Request.HelpfulTools,
            Distractions = request.Request.Distractions
        };

        return RequestResult<PortfolioLearningStyleDto>.Success(learningStyle);
    }
}
