using API.Application.Features.Admin.WeeklyChallenges.DTOs;
using API.Helpers;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.WeeklyChallenges.Queries;

public record GetWeeklyChallengesQuery : IRequest<RequestResult<List<WeeklyChallengeDto>>>;

public class GetWeeklyChallengesQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.Teacher.WeeklyChallenges> challengesRepository)
    : RequestHandlerBase<GetWeeklyChallengesQuery, RequestResult<List<WeeklyChallengeDto>>>(parameters)
{
    public override async Task<RequestResult<List<WeeklyChallengeDto>>> Handle(GetWeeklyChallengesQuery request, CancellationToken cancellationToken)
    {
        var challenges = await challengesRepository.Get()
            .OrderByDescending(c => c.WeekNumber)
            .Select(c => new WeeklyChallengeDto
            {
                Id = c.ID,
                WeekNumber = c.WeekNumber,
                Title = c.Title,
                Description = c.Description,
                // ResourceLinks = c.ResourceLinks != null ? c.ResourceLinks.Split(',').ToList() : new List<string>(),
                TutorialVideo = c.TutorialVideo,
                SubmissionFormLink = c.SubmissionFormLink,
                PublishDate = c.CreatedAt,
                Status = c.Status.GetDescription(),
                AutoNotify = c.AutoNotify
            })
            .ToListAsync(cancellationToken);

        return RequestResult<List<WeeklyChallengeDto>>.Success(challenges);
    }
}
