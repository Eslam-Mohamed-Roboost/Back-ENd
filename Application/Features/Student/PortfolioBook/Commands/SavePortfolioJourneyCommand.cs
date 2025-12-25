using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Student.PortfolioBook.Commands;

public record SavePortfolioJourneyCommand(SavePortfolioJourneyRequest Request) : IRequest<RequestResult<PortfolioJourneyEntryDto>>;

public class SavePortfolioJourneyCommandHandler(
    RequestHandlerBaseParameters parameters)
    : RequestHandlerBase<SavePortfolioJourneyCommand, RequestResult<PortfolioJourneyEntryDto>>(parameters)
{
    public override async Task<RequestResult<PortfolioJourneyEntryDto>> Handle(SavePortfolioJourneyCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // TODO: Implement database save when entity is created
        var journeyEntry = new PortfolioJourneyEntryDto
        {
            Id = 1, // Generate ID on create
            Date = request.Request.Date,
            SkillsWorking = request.Request.SkillsWorking,
            EvidenceOfLearning = request.Request.EvidenceOfLearning,
            HowGrown = request.Request.HowGrown,
            NextSteps = request.Request.NextSteps
        };

        return RequestResult<PortfolioJourneyEntryDto>.Success(journeyEntry);
    }
}
