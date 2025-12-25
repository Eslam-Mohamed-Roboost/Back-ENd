using API.Application.Features.Admin.Badges.DTOs;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using BadgesEntity = API.Domain.Entities.Gamification.Badges;

namespace API.Application.Features.Admin.Badges.Commands;

public record CreateBadgeCommand(CreateBadgeDto Dto) : IRequest<RequestResult<long>>;

public class CreateBadgeCommandHandler(RequestHandlerBaseParameters parameters, IRepository<BadgesEntity> repository)
    : RequestHandlerBase<CreateBadgeCommand, RequestResult<long>>(parameters)
{
    public override async Task<RequestResult<long>> Handle(CreateBadgeCommand request, CancellationToken cancellationToken)
    {
        var badge = new BadgesEntity
        {
            Name = request.Dto.Name,
            Description = request.Dto.Description,
            Icon = request.Dto.Icon,
            Color = request.Dto.Color,
            Category = request.Dto.Category,
            TargetRole = request.Dto.TargetRole,
            CpdHours = request.Dto.CpdHours,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var id = repository.Add(badge);
        await repository.SaveChangesAsync();

        return RequestResult<long>.Success(id, "Badge created successfully");
    }
}
