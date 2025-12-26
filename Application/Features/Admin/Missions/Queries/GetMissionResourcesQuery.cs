using API.Application.Features.Admin.Missions.DTOs;
using API.Domain.Entities.Missions;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.Missions.Queries;

public record GetMissionResourcesQuery(long MissionId) : IRequest<RequestResult<List<MissionResourceDto>>>;

public class GetMissionResourcesQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<MissionResources> repository)
    : RequestHandlerBase<GetMissionResourcesQuery, RequestResult<List<MissionResourceDto>>>(parameters)
{
    public override async Task<RequestResult<List<MissionResourceDto>>> Handle(GetMissionResourcesQuery request, CancellationToken cancellationToken)
    {
        var resources = await repository.Get(x => x.MissionId == request.MissionId)
            .OrderBy(x => x.Order)
            .Select(r => new MissionResourceDto
            {
                Id = r.ID,
                MissionId = r.MissionId,
                Type = r.Type,
                Title = r.Title,
                Url = r.Url,
                Description = r.Description,
                Order = r.Order,
                IsRequired = r.IsRequired
            })
            .ToListAsync(cancellationToken);

        return RequestResult<List<MissionResourceDto>>.Success(resources);
    }
}

