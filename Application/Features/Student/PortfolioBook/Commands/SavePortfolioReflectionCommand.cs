using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Domain.Entities.Portfolio;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.PortfolioBook.Commands;

public record SavePortfolioReflectionCommand(SavePortfolioReflectionRequest Request) : IRequest<RequestResult<PortfolioReflectionDto>>;

public class SavePortfolioReflectionCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<PortfolioBookReflection> reflectionRepository)
    : RequestHandlerBase<SavePortfolioReflectionCommand, RequestResult<PortfolioReflectionDto>>(parameters)
{
    public override async Task<RequestResult<PortfolioReflectionDto>> Handle(SavePortfolioReflectionCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;
        var today = DateTime.UtcNow.Date;

        // Check if reflection already exists for today
        var exists = await reflectionRepository.Get(r => 
            r.StudentId == studentId && 
            r.SubjectId == request.Request.SubjectId && 
            r.Date.Date == today)
            .AnyAsync(cancellationToken);

        if (exists)
        {
            return RequestResult<PortfolioReflectionDto>.Failure(
                Domain.Enums.ErrorCode.DailyLimitReached, 
                "You have already submitted a reflection for today.");
        }

        var reflection = new PortfolioBookReflection
        {
            StudentId = studentId,
            SubjectId = request.Request.SubjectId,
            Date = DateTime.UtcNow,
            WeekOf = request.Request.WeekOf, // Keep original date if needed
            WhatLearned = request.Request.WhatLearned,
            BiggestAchievement = request.Request.BiggestAchievement,
            ChallengesFaced = request.Request.ChallengesFaced,
            HelpNeeded = request.Request.HelpNeeded,
            Mood = request.Request.Mood,
            CreatedBy = studentId,
            CreatedAt = DateTime.UtcNow
        };

          reflectionRepository.Add(reflection);
        await reflectionRepository.SaveChangesAsync();

        var result = new PortfolioReflectionDto
        {
            Id = reflection.ID,
            WeekOf = reflection.WeekOf,
            WhatLearned = reflection.WhatLearned,
            BiggestAchievement = reflection.BiggestAchievement,
            ChallengesFaced = reflection.ChallengesFaced,
            HelpNeeded = reflection.HelpNeeded,
            Mood = reflection.Mood
        };

        return RequestResult<PortfolioReflectionDto>.Success(result);
    }
}
