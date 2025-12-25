using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ChallengesEntity = API.Domain.Entities.Gamification.Challenges;
using API.Domain.Enums;

namespace API.Application.Features.Admin.Challenges.Commands;

public record SeedChallengesCommand : IRequest<RequestResult<int>>;

public class SeedChallengesCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<ChallengesEntity> repository)
    : RequestHandlerBase<SeedChallengesCommand, RequestResult<int>>(parameters)
{
    public override async Task<RequestResult<int>> Handle(SeedChallengesCommand request, CancellationToken cancellationToken)
    {
        // If challenges already exist, do nothing (idempotent seed)
        var hasAny = await repository.Get().AnyAsync(cancellationToken);
        if (hasAny)
        {
            return RequestResult<int>.Success(0, "Challenges already seeded");
        }

        var now = DateTime.UtcNow;

        var challenges = new List<ChallengesEntity>
        {
            new()
            {
                Title = "Math Sprint",
                Description = "Solve 5 quick math problems in under 10 minutes.",
                Type = ChallengeType.Quiz,
                Difficulty = DifficultyLevel.Easy,
                EstimatedMinutes = 10,
                Icon = "🧮",
                BackgroundColor = "#22c55e",
                ContentUrl = null,
                Points = 25,
                IsActive = true,
                CreatedAt = now
            },
            new()
            {
                Title = "Science Explorer",
                Description = "Watch a short science video and answer a reflection question.",
                Type = ChallengeType.Video,
                Difficulty = DifficultyLevel.Medium,
                EstimatedMinutes = 20,
                Icon = "🔬",
                BackgroundColor = "#0ea5e9",
                ContentUrl = null,
                Points = 30,
                IsActive = true,
                CreatedAt = now
            },
            new()
            {
                Title = "Reading Marathon",
                Description = "Read an article and summarize the main idea in your own words.",
                Type = ChallengeType.Reading,
                Difficulty = DifficultyLevel.Medium,
                EstimatedMinutes = 25,
                Icon = "📖",
                BackgroundColor = "#a855f7",
                ContentUrl = null,
                Points = 35,
                IsActive = true,
                CreatedAt = now
            },
            new()
            {
                Title = "Creative Writing",
                Description = "Write a short story based on the weekly prompt.",
                Type = ChallengeType.Writing,
                Difficulty = DifficultyLevel.Medium,
                EstimatedMinutes = 30,
                Icon = "✍️",
                BackgroundColor = "#f97316",
                ContentUrl = null,
                Points = 40,
                IsActive = true,
                CreatedAt = now
            }
        };

        foreach (var challenge in challenges)
        {
            repository.Add(challenge);
        }

        await repository.SaveChangesAsync();

        return RequestResult<int>.Success(challenges.Count, "Seed challenges created successfully");
    }
}


