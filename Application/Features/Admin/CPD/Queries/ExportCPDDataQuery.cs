using API.Application.Features.Admin.CPD.DTOs;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.CPD.Queries;

public record ExportCPDDataQuery(string? Format = "csv", DateTime? DateFrom = null, DateTime? DateTo = null) 
    : IRequest<RequestResult<List<CPDExportItemDto>>>;

public class ExportCPDDataQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.User> userRepository,
    IRepository<Domain.Entities.Teacher.TeacherCpdProgress> cpdProgressRepository,
    IRepository<Domain.Entities.Teacher.CpdModules> cpdModulesRepository)
    : RequestHandlerBase<ExportCPDDataQuery, RequestResult<List<CPDExportItemDto>>>(parameters)
{
    public override async Task<RequestResult<List<CPDExportItemDto>>> Handle(ExportCPDDataQuery request, CancellationToken cancellationToken)
    {
        var query = from cp in cpdProgressRepository.Get()
                    join u in userRepository.Get() on cp.TeacherId equals u.ID
                    join m in cpdModulesRepository.Get() on cp.ModuleId equals m.ID
                    where cp.Status == ProgressStatus.Completed
                    select new CPDExportItemDto
                    {
                        TeacherName = u.Name,
                        TeacherEmail = u.Email,
                        ModuleTitle = m.Title,
                        HoursEarned = cp.HoursEarned ?? 0,
                        CompletedAt = cp.CompletedAt,
                        StartedAt = cp.StartedAt
                    };

        if (request.DateFrom.HasValue)
        {
            query = query.Where(x => x.CompletedAt >= request.DateFrom.Value);
        }

        if (request.DateTo.HasValue)
        {
            query = query.Where(x => x.CompletedAt <= request.DateTo.Value);
        }

        var data = await query.OrderByDescending(x => x.CompletedAt).ToListAsync(cancellationToken);
        
        return RequestResult<List<CPDExportItemDto>>.Success(data);
    }
}
