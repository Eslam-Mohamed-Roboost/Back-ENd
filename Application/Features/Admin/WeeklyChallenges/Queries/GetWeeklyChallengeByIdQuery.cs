using API.Application.Features.Admin.WeeklyChallenges.DTOs;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WeeklyChallengeEntity = API.Domain.Entities.Teacher.WeeklyChallenges;

namespace API.Application.Features.Admin.WeeklyChallenges.Queries;

public record GetWeeklyChallengeByIdQuery(long Id) : IRequest<RequestResult<WeeklyChallengeDto>>;

public class GetWeeklyChallengeByIdQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<WeeklyChallengeEntity> repository)
    : RequestHandlerBase<GetWeeklyChallengeByIdQuery, RequestResult<WeeklyChallengeDto>>(parameters)
{
    public override async Task<RequestResult<WeeklyChallengeDto>> Handle(GetWeeklyChallengeByIdQuery request, CancellationToken cancellationToken)
    {
        var challenge = await repository.Get(c => c.ID == request.Id)
            .Select(c => new WeeklyChallengeDto
            {
                Id = c.ID,
                WeekNumber = c.WeekNumber,
                Title = c.Title,
                Description = c.Description,
                ResourceLinks = new List<string>(), // ResourceLinks is JSONB string, needs deserialization
                TutorialVideo = c.TutorialVideo,
                SubmissionFormLink = c.SubmissionFormLink,
                PublishDate = c.PublishedAt ?? DateTime.UtcNow,
                Status = c.Status.ToString(),
                AutoNotify = c.AutoNotify
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (challenge == null)
        {
            return RequestResult<WeeklyChallengeDto>.Failure(ErrorCode.NotFound, "Weekly challenge not found");
        }

        return RequestResult<WeeklyChallengeDto>.Success(challenge);
    }
}

