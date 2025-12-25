using API.Application.Features.Admin.Dashboard.DTOs;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.Dashboard.Queries;

public record GetAdminDashboardQuery : IRequest<RequestResult<AdminDashboardDto>>;

public class GetAdminDashboardQueryHandler(
    RequestHandlerBaseParameters parameters, 
    IRepository<Domain.Entities.User> userRepository)
    : RequestHandlerBase<GetAdminDashboardQuery, RequestResult<AdminDashboardDto>>(parameters)
{
    public override async Task<RequestResult<AdminDashboardDto>> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
    {
        var users = userRepository.Get();

        var totalStudents = await users.CountAsync(u => u.Role == ApplicationRole.Student, cancellationToken);
        var totalTeachers = await users.CountAsync(u => u.Role == ApplicationRole.Teacher, cancellationToken);
        var totalBadgesEarned = await users.SelectMany(u => u.Badges).CountAsync(cancellationToken);

        var topStudents = await users
            .Where(u => u.Role == ApplicationRole.Student)
            .OrderByDescending(u => u.Badges.Count)
            .Take(5)
            .Select(u => new TopStudentDto
            {
                StudentId = u.ID,
                Name = u.Name,
                BadgesCount = u.Badges.Count,
                MissionsCompleted = 0
            })
            .ToListAsync(cancellationToken);

        var dashboard = new AdminDashboardDto
        {
            TotalStudents = totalStudents,
            TotalTeachers = totalTeachers,
            TotalBadgesEarned = totalBadgesEarned,
            TotalMissionsCompleted = 0,
            ActiveUsersThisWeek = 0,
            PortfolioFilesUploaded = 0,
            RecentActivities = new List<RecentActivityDto>(),
            TopStudents = topStudents
        };

        return RequestResult<AdminDashboardDto>.Success(dashboard);
    }
}
