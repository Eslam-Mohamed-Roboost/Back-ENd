using API.Application.Features.Admin.CPD.DTOs;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.CPD.Queries;

public record GetCPDCategoriesQuery : IRequest<RequestResult<List<CPDCategoryDto>>>;

public class GetCPDCategoriesQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.Teacher.TeacherCpdProgress> cpdProgressRepository,
    IRepository<Domain.Entities.Teacher.CpdModules> cpdModulesRepository,
    IRepository<Domain.Entities.Gamification.Badges> badgesRepository)
    : RequestHandlerBase<GetCPDCategoriesQuery, RequestResult<List<CPDCategoryDto>>>(parameters)
{
    public override async Task<RequestResult<List<CPDCategoryDto>>> Handle(GetCPDCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await (
            from cp in cpdProgressRepository.Get()
            join m in cpdModulesRepository.Get() on cp.ModuleId equals m.ID
            join b in badgesRepository.Get() on m.BadgeId equals b.ID into badgeGroup
            from badge in badgeGroup.DefaultIfEmpty()
            where cp.Status == ProgressStatus.Completed
            group cp by (badge != null ? badge.Category.ToString() : "General") into g
            select new CPDCategoryDto
            {
                Category = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(c => c.Count)
            .ToListAsync(cancellationToken);

        return RequestResult<List<CPDCategoryDto>>.Success(categories);
    }
}
