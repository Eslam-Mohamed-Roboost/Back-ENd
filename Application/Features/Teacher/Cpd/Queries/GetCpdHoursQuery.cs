using API.Application.Features.Teacher.Cpd.DTOs;
using API.Domain.Entities.Teacher;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Cpd.Queries;

public record GetCpdHoursQuery() : IRequest<RequestResult<CpdHoursSummaryDto>>;

public class GetCpdHoursQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<CpdModules> cpdModuleRepository,
    IRepository<TeacherCpdProgress> progressRepository)
    : RequestHandlerBase<GetCpdHoursQuery, RequestResult<CpdHoursSummaryDto>>(parameters)
{
    public override async Task<RequestResult<CpdHoursSummaryDto>> Handle(
        GetCpdHoursQuery request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        // Get all progress records for this teacher
        var progressRecords = await progressRepository.Get(p => 
            p.TeacherId == teacherId && 
            !p.IsDeleted &&
            p.Status == ProgressStatus.Completed)
            .ToListAsync(cancellationToken);

        // Get module IDs and fetch modules
        var moduleIds = progressRecords.Select(p => p.ModuleId).Distinct().ToList();
        var modules = await cpdModuleRepository.Get(m => moduleIds.Contains(m.ID))
            .ToDictionaryAsync(m => m.ID, cancellationToken);

        // Calculate total hours from progress records
        var totalHours = progressRecords
            .Where(p => p.HoursEarned.HasValue)
            .Sum(p => p.HoursEarned!.Value);
        
        // Calculate this year hours (modules completed this year)
        var currentYear = DateTime.UtcNow.Year;
        var thisYearHours = progressRecords
            .Where(p => p.CompletedAt.HasValue && 
                       p.CompletedAt.Value.Year == currentYear &&
                       p.HoursEarned.HasValue)
            .Sum(p => p.HoursEarned!.Value);

        // Annual goal (default 20 hours, can be configured)
        var annualGoal = 20.0m;
        var progressPercentage = annualGoal > 0 ? (thisYearHours / annualGoal) * 100 : 0;

        // Get recent activities (last 10 completed modules)
        var recentActivities = progressRecords
            .Where(p => p.CompletedAt.HasValue)
            .OrderByDescending(p => p.CompletedAt)
            .Take(10)
            .Select(p =>
            {
                var module = modules.GetValueOrDefault(p.ModuleId);
                return new CpdHoursEntryDto
                {
                    Id = p.ID,
                    ModuleId = p.ModuleId,
                    ModuleTitle = module?.Title ?? "Unknown Module",
                    HoursEarned = p.HoursEarned ?? (module != null ? module.DurationMinutes / 60.0m : 0),
                    CompletedDate = p.CompletedAt!.Value
                };
            })
            .ToList();

        var summary = new CpdHoursSummaryDto
        {
            TotalHours = totalHours,
            ThisYearHours = thisYearHours,
            AnnualGoal = annualGoal,
            ProgressPercentage = progressPercentage,
            RecentActivities = recentActivities
        };

        return RequestResult<CpdHoursSummaryDto>.Success(summary);
    }
}

