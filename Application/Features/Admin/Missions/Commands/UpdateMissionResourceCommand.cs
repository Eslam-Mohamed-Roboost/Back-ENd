using API.Application.Features.Admin.Missions.DTOs;
using API.Domain.Entities.Missions;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.Missions.Commands;

public record UpdateMissionResourceCommand(long Id, UpdateMissionResourceDto Dto) : IRequest<RequestResult<bool>>;

public class UpdateMissionResourceCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<MissionResources> repository)
    : RequestHandlerBase<UpdateMissionResourceCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(UpdateMissionResourceCommand request, CancellationToken cancellationToken)
    {
        var resource = await repository.Get(x => x.ID == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (resource == null)
            return RequestResult<bool>.Failure(ErrorCode.NotFound, "Resource not found");

        resource.Type = request.Dto.Type;
        resource.Title = request.Dto.Title;
        resource.Url = request.Dto.Url;
        resource.Description = request.Dto.Description;
        resource.Order = request.Dto.Order;
        resource.IsRequired = request.Dto.IsRequired;
        resource.UpdatedAt = DateTime.UtcNow;

        repository.Update(resource);
        await repository.SaveChangesAsync();

        return RequestResult<bool>.Success(true, "Resource updated successfully");
    }
}

