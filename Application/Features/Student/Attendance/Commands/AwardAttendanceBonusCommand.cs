using API.Domain.Entities.Gamification;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.Attendance.Commands;

public record AwardAttendanceBonusCommand : IRequest<RequestResult<AttendanceBonusAwardResult>>;

public class AttendanceBonusAwardResult
{
    public int PointsAwarded { get; set; }
    public List<long> BadgesAwarded { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}

public class AwardAttendanceBonusCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.Academic.StudentAttendance> attendanceRepository,
    IRepository<StudentBadges> studentBadgesRepository,
    IRepository<Domain.Entities.Gamification.Badges> badgesRepository)
    : RequestHandlerBase<AwardAttendanceBonusCommand, RequestResult<AttendanceBonusAwardResult>>(parameters)
{
    public override async Task<RequestResult<AttendanceBonusAwardResult>> Handle(AwardAttendanceBonusCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;
        var now = DateTime.UtcNow;

        // Calculate bonuses
        var calculateCommand = new CalculateAttendanceBonusCommand();
        var calculateHandler = new CalculateAttendanceBonusCommandHandler(parameters, attendanceRepository);
        var bonusResult = await calculateHandler.Handle(calculateCommand, cancellationToken);

        if (!bonusResult.IsSuccess || bonusResult.Data == null)
        {
            return RequestResult<AttendanceBonusAwardResult>.Failure(ErrorCode.InternalError, "Failed to calculate attendance bonus");
        }

        var bonus = bonusResult.Data;
        var badgesAwarded = new List<long>();

        // Award badges
        foreach (var badgeId in bonus.BadgeIds)
        {
            var existingBadge = await studentBadgesRepository.Get(x =>
                x.StudentId == studentId &&
                x.BadgeId == badgeId)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingBadge == null)
            {
                var badge = await badgesRepository.Get(x => x.ID == badgeId && x.IsActive)
                    .FirstOrDefaultAsync(cancellationToken);

                if (badge != null)
                {
                    var studentBadge = new StudentBadges
                    {
                        StudentId = studentId,
                        BadgeId = badgeId,
                        AutoAwarded = true,
                        EarnedDate = now,
                        Status = Status.Approved
                    };
                    studentBadgesRepository.Add(studentBadge);
                    badgesAwarded.Add(badgeId);
                }
            }
        }

        await studentBadgesRepository.SaveChangesAsync(cancellationToken);

        var result = new AttendanceBonusAwardResult
        {
            PointsAwarded = bonus.PointsEarned,
            BadgesAwarded = badgesAwarded,
            Message = bonus.Message
        };

        return RequestResult<AttendanceBonusAwardResult>.Success(result);
    }
}

