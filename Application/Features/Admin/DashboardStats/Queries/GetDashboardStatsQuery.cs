using API.Application.Features.Admin.DashboardStats.DTOs;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.DashboardStats.Queries;

public record GetDashboardStatsQuery : IRequest<RequestResult<DashboardStatsDto>>;

public class GetDashboardStatsQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.User> userRepository)
    : RequestHandlerBase<GetDashboardStatsQuery, RequestResult<DashboardStatsDto>>(parameters)
{
    public override async Task<RequestResult<DashboardStatsDto>> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var users = userRepository.Get();
        var totalUsers = await users.CountAsync(cancellationToken);
        var teacherCount = await users.CountAsync(u => u.Role == ApplicationRole.Teacher, cancellationToken);
        var studentCount = await users.CountAsync(u => u.Role == ApplicationRole.Student, cancellationToken);
        var totalBadges = await users.SelectMany(u => u.Badges).CountAsync(cancellationToken);

        var result = new DashboardStatsDto
        {
            Stats = new List<StatsCardDto>
            {
                new()
                {
                    Title = "Total Users",
                    Value = totalUsers,
                    Breakdown = $"{teacherCount} Teachers • {studentCount} Students",
                    Icon = "👥",
                    Trend = "neutral"
                },
                new()
                {
                    Title = "Badges Earned",
                    Value = totalBadges,
                    Breakdown = $"{totalBadges} Total Badges",
                    Comparison = "↑ 12 from last week",
                    Icon = "🏆",
                    Trend = "up"
                },
                new()
                {
                    Title = "This Week Activity",
                    Value = 1250,
                    Breakdown = "Logins, Submissions, Completions",
                    Icon = "📊",
                    Trend = "up"
                }
            },
            PendingApprovals = 0
        };

        return RequestResult<DashboardStatsDto>.Success(result);
    }
}
