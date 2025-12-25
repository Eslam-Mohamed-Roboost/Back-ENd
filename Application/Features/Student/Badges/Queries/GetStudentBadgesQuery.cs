using API.Application.Features.Student.Badges.DTOs;
using API.Application.Features.Student.Portfolio.DTOs;
using API.Domain.Entities.Gamification;
using API.Helpers;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.Badges.Queries;

public record GetStudentBadgesQuery : IRequest<RequestResult<StudentBadgesSummaryDto>>;

public class GetStudentBadgesQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.Gamification.Badges> badgesRepository,
    IRepository<StudentBadges> studentBadgesRepository,
    IRepository<StudentLevels> studentLevelsRepository)
    : RequestHandlerBase<GetStudentBadgesQuery, RequestResult<StudentBadgesSummaryDto>>(parameters)
{
    public override async Task<RequestResult<StudentBadgesSummaryDto>> Handle(GetStudentBadgesQuery request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // Get all badges
        var allBadges = await badgesRepository.Get(x => x.IsActive).ToListAsync(cancellationToken);
        
        // Get student's earned badges
        var earnedBadgesMap = await (from sb in studentBadgesRepository.Get()
                                      where sb.StudentId == studentId && sb.Status == Status.Approved
                                      select sb)
            .ToDictionaryAsync(x => x.BadgeId, cancellationToken);

        // Get student level info
        var studentLevel = await studentLevelsRepository.Get(x => x.StudentId == studentId)
            .FirstOrDefaultAsync(cancellationToken);

        var badgeDtos = allBadges.Select(b => new BadgeDto
        {
            Id = b.ID,
            Name = b.Name,
            Icon = b.Icon ?? "🏆",
            Earned = earnedBadgesMap.ContainsKey(b.ID),
            EarnDate = earnedBadgesMap.ContainsKey(b.ID) ? earnedBadgesMap[b.ID].EarnedDate : null,
            Requirement = b.Description ?? "Complete specific tasks to earn",
            Category = b.Category.ToString()
        }).ToList();

        var portfolioBadges = allBadges
            .Where(b => earnedBadgesMap.ContainsKey(b.ID))
            .Select(b => new PortfolioBadgeDto
            {
                Id = b.ID,
                Name = b.Name,
                Description = b.Description ?? "",
                Icon = b.Icon ?? "🏆",
                Color = b.Color ?? "#FFD700",
                EarnedDate = earnedBadgesMap[b.ID].EarnedDate,
                Category = b.Category.ToString()
            }).ToList();

        var summary = new StudentBadgesSummaryDto
        {
            TotalBadges = allBadges.Count,
            EarnedBadges = earnedBadgesMap.Count,
            LockedBadges = allBadges.Count - earnedBadgesMap.Count,
            CurrentLevel = studentLevel?.LevelName.GetDescription() ?? "Digital Scout",
            NextLevel = GetNextLevel(studentLevel?.LevelName.GetDescription()?? "Digital Scout"),
            BadgesUntilNextLevel = CalculateBadgesUntilNextLevel(earnedBadgesMap.Count),
            Badges = badgeDtos,
            PortfolioBadges = portfolioBadges
        };

        return RequestResult<StudentBadgesSummaryDto>.Success(summary);
    }

    private static string GetNextLevel(string currentLevel)
    {
        return currentLevel switch
        {
            "Digital Scout" => "Digital Explorer",
            "Digital Explorer" => "Digital Pioneer",
            "Digital Pioneer" => "Digital Master",
            "Digital Master" => "Digital Legend",
            _ => "Digital Explorer"
        };
    }

    private static int CalculateBadgesUntilNextLevel(int currentBadges)
    {
        // Simple level progression: every 5 badges = next level
        var nextLevelThreshold = ((currentBadges / 5) + 1) * 5;
        return nextLevelThreshold - currentBadges;
    }
}
