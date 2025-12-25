using API.Application.Features.Admin.BadgeSubmissions.DTOs;
using API.Domain.Entities.Teacher;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.BadgeSubmissions.Queries;

public record GetBadgeSubmissionByIdQuery(long Id) : IRequest<RequestResult<BadgeSubmissionDto>>;

public class GetBadgeSubmissionByIdQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<TeacherBadgeSubmissions> repository)
    : RequestHandlerBase<GetBadgeSubmissionByIdQuery, RequestResult<BadgeSubmissionDto>>(parameters)
{
    public override async Task<RequestResult<BadgeSubmissionDto>> Handle(GetBadgeSubmissionByIdQuery request, CancellationToken cancellationToken)
    {
        var submission = await repository.Get(s => s.ID == request.Id)
            .Select(s => new BadgeSubmissionDto
            {
                Id = s.ID,
                UserId = s.TeacherId,
                UserName = "", // Teacher navigation not included in entity
                UserRole = 2, // Teacher role
                UserAvatar = null,
                BadgeId = s.BadgeId,
                BadgeName = "", // Badge navigation not included in entity
                BadgeIcon = "",
                BadgeCategory = "",
                CpdHours = s.CpdHoursAwarded,
                EvidenceLink = s.EvidenceLink,
                SubmitterNotes = s.SubmitterNotes,
                SubmissionDate = s.SubmittedAt,
                Status = s.Status.ToString(),
                ReviewedBy = s.ReviewedBy,
                ReviewDate = s.ReviewedAt,
                ReviewNotes = s.ReviewNotes
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (submission == null)
        {
            return RequestResult<BadgeSubmissionDto>.Failure(ErrorCode.NotFound, "Badge submission not found");
        }

        return RequestResult<BadgeSubmissionDto>.Success(submission);
    }
}

