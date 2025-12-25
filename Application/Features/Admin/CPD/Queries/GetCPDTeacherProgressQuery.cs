using API.Application.Features.Admin.CPD.DTOs;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.CPD.Queries;

public record GetCPDTeacherProgressQuery(string? SortBy = null, string? SortDirection = "asc") 
    : IRequest<RequestResult<List<TeacherCPDProgressDto>>>;

public class GetCPDTeacherProgressQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.User> userRepository,
    IRepository<Domain.Entities.Teacher.TeacherCpdProgress> cpdProgressRepository,
    IRepository<Domain.Entities.Teacher.CpdModules> cpdModulesRepository,
    IRepository<Domain.Entities.Gamification.Badges> badgesRepository)
    : RequestHandlerBase<GetCPDTeacherProgressQuery, RequestResult<List<TeacherCPDProgressDto>>>(parameters)
{
    public override async Task<RequestResult<List<TeacherCPDProgressDto>>> Handle(GetCPDTeacherProgressQuery request, CancellationToken cancellationToken)
    {
        // Get all teachers
        var teachers = await userRepository.Get()
            .Where(u => u.Role == ApplicationRole.Teacher)
            .Select(u => new
            {
                u.ID,
                u.Name,
                u.Email
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var teacherIds = teachers.Select(t => t.ID).ToList();

        // Get CPD progress with module and badge details
        var cpdData = await (
            from cp in cpdProgressRepository.Get()
            join m in cpdModulesRepository.Get() on cp.ModuleId equals m.ID
            join b in badgesRepository.Get() on m.BadgeId equals b.ID into badgeGroup
            from badge in badgeGroup.DefaultIfEmpty()
            where teacherIds.Contains(cp.TeacherId) && cp.Status == ProgressStatus.Completed
            select new
            {
                cp.TeacherId,
                cp.HoursEarned,
                cp.CompletedAt,
                BadgeCategory = badge != null ? badge.Category.ToString() : "Unknown"
            })
            .ToListAsync(cancellationToken);

        // Build result
        var result = teachers.Select(teacher =>
        {
            var teacherCpdData = cpdData.Where(c => c.TeacherId == teacher.ID).ToList();

            return new TeacherCPDProgressDto
            {
                Id = teacher.ID,
                Name = teacher.Name,
                Email = teacher.Email,
                BadgeCount = teacherCpdData.Count,
                CpdHours = (int)teacherCpdData.Sum(c => c.HoursEarned ?? 0),
                LastBadgeDate = teacherCpdData.Any() 
                    ? teacherCpdData.Max(c => c.CompletedAt) 
                    : null,
                Categories = teacherCpdData
                    .Select(c => c.BadgeCategory)
                    .Distinct()
                    .ToList()
            };
        }).ToList();

        // Apply sorting
        result = ApplySorting(result, request.SortBy, request.SortDirection);

        return RequestResult<List<TeacherCPDProgressDto>>.Success(result);
    }

    private List<TeacherCPDProgressDto> ApplySorting(List<TeacherCPDProgressDto> data, string? sortBy, string? sortDirection)
    {
        var isDescending = sortDirection?.ToLower() == "desc";

        return sortBy?.ToLower() switch
        {
            "name" => isDescending 
                ? data.OrderByDescending(t => t.Name).ToList()
                : data.OrderBy(t => t.Name).ToList(),
            
            "badgecount" or "badges" => isDescending
                ? data.OrderByDescending(t => t.BadgeCount).ToList()
                : data.OrderBy(t => t.BadgeCount).ToList(),
            
            "cpdhours" or "hours" => isDescending
                ? data.OrderByDescending(t => t.CpdHours).ToList()
                : data.OrderBy(t => t.CpdHours).ToList(),
            
            "lastbadgedate" or "lastbadge" => isDescending
                ? data.OrderByDescending(t => t.LastBadgeDate ?? DateTime.MinValue).ToList()
                : data.OrderBy(t => t.LastBadgeDate ?? DateTime.MinValue).ToList(),
            
            _ => data.OrderByDescending(t => t.CpdHours).ToList() // Default: sort by hours descending
        };
    }
}
