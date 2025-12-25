using API.Application.Features.Admin.WeeklyChallenges.DTOs;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using System.Text.Json;
using WeeklyChallengesEntity = API.Domain.Entities.Teacher.WeeklyChallenges;

namespace API.Application.Features.Admin.WeeklyChallenges.Commands;

public record CreateWeeklyChallengeCommand(CreateWeeklyChallengeDto Dto) : IRequest<RequestResult<long>>;

public class CreateWeeklyChallengeCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<WeeklyChallengesEntity> repository)
    : RequestHandlerBase<CreateWeeklyChallengeCommand, RequestResult<long>>(parameters)
{
    public override async Task<RequestResult<long>> Handle(CreateWeeklyChallengeCommand request, CancellationToken cancellationToken)
    {
        // Parse the status string to enum
        if (!Enum.TryParse<PublishStatus>(request.Dto.Status, true, out var status))
        {
            status = PublishStatus.Draft;
        }

        // Serialize ResourceLinks to JSON
        var resourceLinksJson = request.Dto.ResourceLinks != null && request.Dto.ResourceLinks.Count > 0
            ? JsonSerializer.Serialize(request.Dto.ResourceLinks)
            : null;

        var weeklyChallenge = new WeeklyChallengesEntity
        {
            WeekNumber = request.Dto.WeekNumber,
            Title = request.Dto.Title,
            Description = request.Dto.Description,
            ResourceLinks = resourceLinksJson,
            TutorialVideo = request.Dto.TutorialVideo,
            SubmissionFormLink = request.Dto.SubmissionFormLink,
            Status = status,
            PublishedAt = status == PublishStatus.Published ? DateTime.UtcNow : null,
            AutoNotify = request.Dto.AutoNotify,
            CreatedAt = DateTime.UtcNow
        };

        var id = repository.Add(weeklyChallenge);
        await repository.SaveChangesAsync();

        return RequestResult<long>.Success(id, "Weekly challenge created successfully");
    }
}

