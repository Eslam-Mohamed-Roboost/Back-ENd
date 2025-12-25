using API.Application.Features.Student.Portfolio.DTOs;
using API.Application.Features.Teacher.Portfolio.DTOs;
using API.Domain.Entities.Gamification;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BadgeEntity = API.Domain.Entities.Gamification.Badges;

namespace API.Application.Features.Teacher.Portfolio.Commands;

public record AwardPortfolioBadgeCommand(long StudentId, long SubjectId, TeacherAwardPortfolioBadgeRequest Request) : IRequest<RequestResult<PortfolioBadgeDto>>;

public class AwardPortfolioBadgeCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<BadgeEntity> badgesRepository,
    IRepository<StudentBadges> studentBadgesRepository)
    : RequestHandlerBase<AwardPortfolioBadgeCommand, RequestResult<PortfolioBadgeDto>>(parameters)
{
    public override async Task<RequestResult<PortfolioBadgeDto>> Handle(AwardPortfolioBadgeCommand request, CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;
        var studentId = request.StudentId;

        var badge = await badgesRepository.Get(x => x.ID == request.Request.BadgeId && x.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        if (badge == null)
            return RequestResult<PortfolioBadgeDto>.Failure(ErrorCode.NotFound, "Badge not found");

        var existing = await studentBadgesRepository.Get(x =>
                x.StudentId == studentId &&
                x.BadgeId == request.Request.BadgeId)
            .FirstOrDefaultAsync(cancellationToken);

        if (existing != null)
        {
            return RequestResult<PortfolioBadgeDto>.Failure(ErrorCode.BadRequest, "Badge already awarded");
        }

        var now = DateTime.UtcNow;

        var studentBadge = new StudentBadges
        {
            StudentId = studentId,
            BadgeId = request.Request.BadgeId,
            MissionId = null,
            AutoAwarded = false,
            EarnedDate = now,
            Status = Status.Approved
        };

        studentBadgesRepository.Add(studentBadge);
        await studentBadgesRepository.SaveChangesAsync();

        var dto = new PortfolioBadgeDto
        {
            Id = badge.ID,
            Name = badge.Name,
            Description = badge.Description ?? string.Empty,
            Icon = badge.Icon ?? string.Empty,
            Color = badge.Color ?? "#6366f1",
            EarnedDate = now,
            RelatedWorkId = null,
            Category = badge.Category.ToString()
        };

        return RequestResult<PortfolioBadgeDto>.Success(dto);
    }
}


