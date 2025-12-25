using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MissionsEntity = API.Domain.Entities.Missions.Missions;

namespace API.Application.Features.Admin.Missions.Commands;

public record SeedMissionsCommand : IRequest<RequestResult<int>>;

public class SeedMissionsCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<MissionsEntity> repository)
    : RequestHandlerBase<SeedMissionsCommand, RequestResult<int>>(parameters)
{
    public override async Task<RequestResult<int>> Handle(SeedMissionsCommand request, CancellationToken cancellationToken)
    {
        // If missions already exist, do nothing (idempotent seed)
        var hasAny = await repository.Get().AnyAsync(cancellationToken);
        if (hasAny)
        {
            return RequestResult<int>.Success(0, "Missions already seeded");
        }

        var now = DateTime.UtcNow;

        var missions = new List<MissionsEntity>
        {
            new()
            {
                Number = 1,
                Title = "Welcome to SchoolHub",
                Description = "Complete your first mission and explore your student dashboard.",
                Icon = "🚀",
                EstimatedMinutes = 15,
                BadgeId = 0,
                Order = 1,
                IsEnabled = true,
                CreatedAt = now
            },
            new()
            {
                Number = 2,
                Title = "Portfolio Starter",
                Description = "Upload your first piece of work to your digital portfolio.",
                Icon = "📁",
                EstimatedMinutes = 20,
                BadgeId = 0,
                Order = 2,
                IsEnabled = true,
                CreatedAt = now
            },
            new()
            {
                Number = 3,
                Title = "Reflection Time",
                Description = "Write a short reflection about something you learned this week.",
                Icon = "📝",
                EstimatedMinutes = 20,
                BadgeId = 0,
                Order = 3,
                IsEnabled = true,
                CreatedAt = now
            },
            new()
            {
                Number = 4,
                Title = "Mission Explorer",
                Description = "Complete at least one activity inside a learning mission.",
                Icon = "🧭",
                EstimatedMinutes = 30,
                BadgeId = 0,
                Order = 4,
                IsEnabled = true,
                CreatedAt = now
            },
            new()
            {
                Number = 5,
                Title = "Badge Hunter",
                Description = "Earn your first badge by completing a mission or challenge.",
                Icon = "🏅",
                EstimatedMinutes = 30,
                BadgeId = 0,
                Order = 5,
                IsEnabled = true,
                CreatedAt = now
            },
            new()
            {
                Number = 6,
                Title = "Challenge Accepted",
                Description = "Join a weekly challenge and submit your answer.",
                Icon = "⚔️",
                EstimatedMinutes = 25,
                BadgeId = 0,
                Order = 6,
                IsEnabled = true,
                CreatedAt = now
            },
            new()
            {
                Number = 7,
                Title = "Notebook Pro",
                Description = "Create at least two notes in your digital notebook.",
                Icon = "📓",
                EstimatedMinutes = 20,
                BadgeId = 0,
                Order = 7,
                IsEnabled = true,
                CreatedAt = now
            },
            new()
            {
                Number = 8,
                Title = "Progress Tracker",
                Description = "Review your progress dashboard and set a new goal.",
                Icon = "📊",
                EstimatedMinutes = 15,
                BadgeId = 0,
                Order = 8,
                IsEnabled = true,
                CreatedAt = now
            }
        };

        foreach (var mission in missions)
        {
            repository.Add(mission);
        }

        await repository.SaveChangesAsync();

        return RequestResult<int>.Success(missions.Count, "8 missions seeded successfully");
    }
}


