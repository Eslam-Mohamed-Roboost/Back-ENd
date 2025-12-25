using API.Application.Features.Admin.Portfolio.DTOs;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.Portfolio.Queries;

public record GetPortfolioRecentUpdatesQuery(int Limit = 10) : IRequest<RequestResult<List<PortfolioUpdateDto>>>;

public class GetPortfolioRecentUpdatesQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.Portfolio.PortfolioFiles> portfolioRepository,
    IRepository<Domain.Entities.User> userRepository,
    IRepository<Domain.Entities.General.Classes> classRepository,
    IRepository<Domain.Entities.General.Subjects> subjectRepository)
    : RequestHandlerBase<GetPortfolioRecentUpdatesQuery, RequestResult<List<PortfolioUpdateDto>>>(parameters)
{
    public override async Task<RequestResult<List<PortfolioUpdateDto>>> Handle(GetPortfolioRecentUpdatesQuery request, CancellationToken cancellationToken)
    {
        var recentUpdates = await (
            from pf in portfolioRepository.Get()
            join u in userRepository.Get() on pf.StudentId equals u.ID
            join c in classRepository.Get() on u.ClassID equals c.ID into classGroup
            from cls in classGroup.DefaultIfEmpty()
            join s in subjectRepository.Get() on pf.SubjectId equals s.ID into subjectGroup
            from subj in subjectGroup.DefaultIfEmpty()
            orderby pf.UploadedAt descending
            select new PortfolioUpdateDto
            {
                StudentId = pf.StudentId,
                StudentName = u.Name,
                ClassName = cls != null ? cls.Name : "No Class",
                Subject = subj != null ? subj.Name : "Unknown",
                UpdateType = "upload",
                Timestamp = pf.UploadedAt,
                ItemCount = 1
            })
            .Take(request.Limit)
            .ToListAsync(cancellationToken);

        return RequestResult<List<PortfolioUpdateDto>>.Success(recentUpdates);
    }
}
