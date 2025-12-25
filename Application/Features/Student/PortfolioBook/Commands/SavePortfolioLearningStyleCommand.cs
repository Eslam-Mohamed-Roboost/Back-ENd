using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Student.PortfolioBook.Commands;

public record SavePortfolioLearningStyleCommand(SavePortfolioLearningStyleRequest Request) : IRequest<RequestResult<PortfolioLearningStyleDto>>;

public class SavePortfolioLearningStyleCommandHandler(
    RequestHandlerBaseParameters parameters)
    : RequestHandlerBase<SavePortfolioLearningStyleCommand, RequestResult<PortfolioLearningStyleDto>>(parameters)
{
    public override async Task<RequestResult<PortfolioLearningStyleDto>> Handle(SavePortfolioLearningStyleCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // TODO: Implement database save when entity is created
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
