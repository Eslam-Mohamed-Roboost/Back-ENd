using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Student.PortfolioBook.Commands;

public record SavePortfolioReflectionCommand(SavePortfolioReflectionRequest Request) : IRequest<RequestResult<PortfolioReflectionDto>>;

public class SavePortfolioReflectionCommandHandler(
    RequestHandlerBaseParameters parameters)
    : RequestHandlerBase<SavePortfolioReflectionCommand, RequestResult<PortfolioReflectionDto>>(parameters)
{
    public override async Task<RequestResult<PortfolioReflectionDto>> Handle(SavePortfolioReflectionCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // TODO: Implement database save when entity is created
        var reflection = new PortfolioReflectionDto
        {
            Id = 1, // Generate ID on create
            WeekOf = request.Request.WeekOf,
            WhatLearned = request.Request.WhatLearned,
            BiggestAchievement = request.Request.BiggestAchievement,
            ChallengesFaced = request.Request.ChallengesFaced,
            HelpNeeded = request.Request.HelpNeeded,
            Mood = request.Request.Mood
        };

        return RequestResult<PortfolioReflectionDto>.Success(reflection);
    }
}
