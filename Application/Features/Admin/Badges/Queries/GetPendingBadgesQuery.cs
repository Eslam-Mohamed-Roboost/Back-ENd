using API.Domain.Entities.Gamification;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BadgeEntity = API.Domain.Entities.Users.Badges;

namespace API.Application.Features.Admin.Badges.Queries;

public record GetPendingBadgesQuery(
    long? ClassId = null,
    long? StudentId = null,
    long? BadgeId = null) : IRequest<RequestResult<List<PendingBadgeDto>>>;

public class PendingBadgeDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public long BadgeId { get; set; }
    public string BadgeName { get; set; } = string.Empty;
    public DateTime EarnedAt { get; set; }
    public bool IsApproved { get; set; }
}

public class GetPendingBadgesQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<StudentBadges> studentBadgeRepository,
    IRepository<BadgeEntity> badgeRepository,
    IRepository<Domain.Entities.User> userRepository)
    : RequestHandlerBase<GetPendingBadgesQuery, RequestResult<List<PendingBadgeDto>>>(parameters)
{
    public override async Task<RequestResult<List<PendingBadgeDto>>> Handle(
        GetPendingBadgesQuery request,
        CancellationToken cancellationToken)
    {
        // Get all student badges (assuming no explicit approval status, we return all recent ones)
        var query = studentBadgeRepository.Get();

        if (request.StudentId.HasValue)
        {
            query = query.Where(sb => sb.StudentId == request.StudentId.Value);
        }

        if (request.BadgeId.HasValue)
        {
            query = query.Where(sb => sb.BadgeId == request.BadgeId.Value);
        }

        var studentBadges = await query
            .OrderByDescending(sb => sb.EarnedDate)
            .Take(100)
            .ToListAsync(cancellationToken);

        var pendingBadges = new List<PendingBadgeDto>();

        foreach (var sb in studentBadges)
        {
            var student = await userRepository.Get(u => u.ID == sb.StudentId)
                .FirstOrDefaultAsync(cancellationToken);

            var badge = await badgeRepository.Get(b => b.ID == sb.BadgeId)
                .FirstOrDefaultAsync(cancellationToken);

            if (student != null && badge != null)
            {
                var className = "No Class";
                if (student.ClassID.HasValue)
                {
                    var classEntity = await userRepository.Get(u => u.ID == student.ClassID.Value)
                        .Select(u => u.Classes)
                        .FirstOrDefaultAsync(cancellationToken);
                    className = classEntity?.Name ?? "No Class";
                }

                // Filter by class if requested
                if (request.ClassId.HasValue && student.ClassID != request.ClassId.Value)
                {
                    continue;
                }

                pendingBadges.Add(new PendingBadgeDto
                {
                    Id = sb.ID,
                    StudentId = sb.StudentId,
                    StudentName = student.Name,
                    ClassName = className,
                    BadgeId = sb.BadgeId,
                    BadgeName = badge.Name,
                    EarnedAt = sb.EarnedDate,
                    IsApproved = sb.Status == Status.Approved
                });
            }
        }

        return RequestResult<List<PendingBadgeDto>>.Success(pendingBadges);
    }
}

