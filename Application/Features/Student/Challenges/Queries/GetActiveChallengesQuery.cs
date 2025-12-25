using API.Application.Features.Student.Challenges.DTOs;
using API.Domain.Entities.Gamification;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using API.Helpers;

namespace API.Application.Features.Student.Challenges.Queries;

public record GetActiveChallengesQuery : IRequest<RequestResult<List<ChallengeDto>>>;

public class GetActiveChallengesQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.Gamification.Challenges> challengesRepository,
    IRepository<StudentChallenges> studentChallengesRepository)
    : RequestHandlerBase<GetActiveChallengesQuery, RequestResult<List<ChallengeDto>>>(parameters)
{
    public override async Task<RequestResult<List<ChallengeDto>>> Handle(GetActiveChallengesQuery request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;
        var now = DateTime.UtcNow;

        // Get active challenges
        var challenges = await challengesRepository.Get(x => x.IsActive).ToListAsync(cancellationToken);

        // Get student's challenge progress
        var studentChallengeMap = await studentChallengesRepository.Get(x => x.StudentId == studentId)
            .ToDictionaryAsync(x => x.ChallengeId, cancellationToken);

        // Get participant counts
        var participantCounts = await studentChallengesRepository.Get()
            .GroupBy(x => x.ChallengeId)
            .Select(g => new { ChallengeId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.ChallengeId, x => x.Count, cancellationToken);

        var result = challenges.Select(c =>
        {
            var studentChallenge = studentChallengeMap.GetValueOrDefault(c.ID);
            
            return new ChallengeDto
            {
                Id = c.ID,
                Title = c.Title,
                Description = c.Description ?? "",
                Icon = c.Icon ?? "🎯",
                StartDate = c.CreatedAt, // Using creation date as start
                EndDate = c.CreatedAt.AddDays(7), // Weekly challenges
                Difficulty = c.Difficulty.GetDescription(),
                Points = c.Points,
                Completed = studentChallenge?.Status == ProgressStatus.Completed,
                ParticipantCount = participantCounts.GetValueOrDefault(c.ID, 0),
                Tags = new List<string> { c.Type.GetDescription() }
            };
        }).ToList();

        return RequestResult<List<ChallengeDto>>.Success(result);
    }
}
