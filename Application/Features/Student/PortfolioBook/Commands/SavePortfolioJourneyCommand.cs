using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Domain.Entities.Portfolio;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.PortfolioBook.Commands;

public record SavePortfolioJourneyCommand(SavePortfolioJourneyRequest Request) : IRequest<RequestResult<PortfolioJourneyEntryDto>>;

public class SavePortfolioJourneyCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<PortfolioBookJourneyEntry> journeyRepository)
    : RequestHandlerBase<SavePortfolioJourneyCommand, RequestResult<PortfolioJourneyEntryDto>>(parameters)
{
    public override async Task<RequestResult<PortfolioJourneyEntryDto>> Handle(SavePortfolioJourneyCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;
        var today = DateTime.UtcNow.Date;

        // Check if journey entry already exists for today
        var exists = await journeyRepository.Get(j => 
            j.StudentId == studentId && 
            j.SubjectId == request.Request.SubjectId && 
            j.Date.Date == today)
            .AnyAsync(cancellationToken);

        if (exists)
        {
            return RequestResult<PortfolioJourneyEntryDto>.Failure(
                Domain.Enums.ErrorCode.DailyLimitReached, 
                "You have already submitted a journey entry for today.");
        }

        var journeyEntry = new PortfolioBookJourneyEntry
        {
            StudentId = studentId,
            SubjectId = request.Request.SubjectId,
            Date = DateTime.UtcNow,
            SkillsWorking = request.Request.SkillsWorking,
            EvidenceOfLearning = request.Request.EvidenceOfLearning,
            HowGrown = request.Request.HowGrown,
            NextSteps = request.Request.NextSteps,
            CreatedBy = studentId,
            CreatedAt = DateTime.UtcNow
        };

         journeyRepository.Add(journeyEntry);
        await journeyRepository.SaveChangesAsync();

        var result = new PortfolioJourneyEntryDto
        {
            Id = journeyEntry.ID,
            Date = journeyEntry.Date,
            SkillsWorking = journeyEntry.SkillsWorking,
            EvidenceOfLearning = journeyEntry.EvidenceOfLearning,
            HowGrown = journeyEntry.HowGrown,
            NextSteps = journeyEntry.NextSteps
        };

        return RequestResult<PortfolioJourneyEntryDto>.Success(result);
    }
}
