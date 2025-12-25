using API.Application.Features.Student.Activity.DTOs;
using API.Domain.Entities.Gamification;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using API.Helpers;

namespace API.Application.Features.Student.Activity.Queries;

public record GetPointsSummaryQuery : IRequest<RequestResult<PointsSummaryDto>>;

public class GetPointsSummaryQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<StudentChallenges> challengesRepository,
    IRepository<StudentLevels> studentLevelsRepository)
    : RequestHandlerBase<GetPointsSummaryQuery, RequestResult<PointsSummaryDto>>(parameters)
{
    public override async Task<RequestResult<PointsSummaryDto>> Handle(GetPointsSummaryQuery request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // Get total points from challenges
        var totalPoints = await challengesRepository.Get(x => x.StudentId == studentId)
            .SumAsync(x => x.PointsEarned, cancellationToken);

        // Get points this week
        var weekStart = DateTime.UtcNow.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
        var pointsThisWeek = await challengesRepository.Get(x => 
                x.StudentId == studentId && 
                x.CompletedAt.HasValue && 
                x.CompletedAt.Value >= weekStart)
            .SumAsync(x => x.PointsEarned, cancellationToken);

        // Get points this month
        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var pointsThisMonth = await challengesRepository.Get(x => 
                x.StudentId == studentId && 
                x.CompletedAt.HasValue && 
                x.CompletedAt.Value >= monthStart)
            .SumAsync(x => x.PointsEarned, cancellationToken);

        // Get student level
        var studentLevel = await studentLevelsRepository.Get(x => x.StudentId == studentId)
            .FirstOrDefaultAsync(cancellationToken);

        var currentLevel = studentLevel?.LevelName.GetDescription() ?? "Digital Scout";
        var pointsToNextLevel = 100 - (totalPoints % 100);

        // Get recent earnings
        var recentEarnings = await challengesRepository.Get(x => x.StudentId == studentId && x.CompletedAt.HasValue)
            .OrderByDescending(x => x.CompletedAt)
            .Take(10)
            .Select(x => new PointsHistoryDto
            {
                Date = x.CompletedAt!.Value,
                Points = x.PointsEarned,
                Source = "Challenge Completed",
                Description = "Earned points from completing a challenge"
            })
            .ToListAsync(cancellationToken);

        var summary = new PointsSummaryDto
        {
            TotalPoints = totalPoints,
            PointsThisWeek = pointsThisWeek,
            PointsThisMonth = pointsThisMonth,
            CurrentLevel = currentLevel,
            PointsToNextLevel = pointsToNextLevel,
            RecentEarnings = recentEarnings,
            Breakdown = new PointsBreakdownDto
            {
                FromMissions = 0, // TODO: Calculate from missions
                FromChallenges = totalPoints,
                FromBadges = 0, // TODO: Calculate from badges
                FromPortfolio = 0, // TODO: Calculate from portfolio
                FromStreak = 0 // TODO: Calculate from streaks
            }
        };

        return RequestResult<PointsSummaryDto>.Success(summary);
    }
}
