using API.Application.Features.Admin.Badges.DTOs;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BadgesEntity = API.Domain.Entities.Gamification.Badges;

namespace API.Application.Features.Admin.Badges.Commands;

public record UpdateBadgeCommand(long Id, UpdateBadgeDto Dto) : IRequest<RequestResult<bool>>;

public class UpdateBadgeCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<BadgesEntity> repository)
    : RequestHandlerBase<UpdateBadgeCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(UpdateBadgeCommand request, CancellationToken cancellationToken)
    {
        var badge = await repository.Get()
            .FirstOrDefaultAsync(b => b.ID == request.Id, cancellationToken);

        if (badge == null)
        {
            return RequestResult<bool>.Failure(ErrorCode.NotFound, "Badge not found");
        }

        // Update badge properties
        badge.Name = request.Dto.Name;
        badge.Description = request.Dto.Description;
        badge.Icon = request.Dto.Icon;
        badge.Color = request.Dto.Color;
        badge.Category = (BadgeCategory)request.Dto.Category;
        badge.TargetRole = (BadgeTargetRole)request.Dto.TargetRole;
        badge.CpdHours = request.Dto.CpdHours;
        badge.IsActive = request.Dto.IsActive;
        badge.UpdatedAt = DateTime.UtcNow;

        repository.Update(badge);
        await repository.SaveChangesAsync();

        return RequestResult<bool>.Success(true, "Badge updated successfully");
    }
}

