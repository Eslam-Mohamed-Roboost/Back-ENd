using API.Domain.Entities.Gamification;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.Badges.Queries;

public record GetStudentBadgesCountQuery : IRequest<RequestResult<int>>;

public class GetStudentBadgesCountQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<StudentBadges> studentBadgesRepository)
    : RequestHandlerBase<GetStudentBadgesCountQuery, RequestResult<int>>(parameters)
{
    public override async Task<RequestResult<int>> Handle(GetStudentBadgesCountQuery request, CancellationToken cancellationToken)
    {
        var count = await studentBadgesRepository.Get(x => x.StudentId == _userState.UserID)
            .CountAsync(cancellationToken);

        return RequestResult<int>.Success(count);
    }
}
