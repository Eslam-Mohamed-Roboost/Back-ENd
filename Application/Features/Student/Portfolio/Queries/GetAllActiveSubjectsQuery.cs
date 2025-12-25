using API.Application.Features.Student.Portfolio.DTOs;
using API.Domain.Entities.General;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.Portfolio.Queries;

public record GetAllActiveSubjectsQuery : IRequest<RequestResult<List<SimpleSubjectDto>>>;

public class GetAllActiveSubjectsQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Subjects> subjectsRepository)
    : RequestHandlerBase<GetAllActiveSubjectsQuery, RequestResult<List<SimpleSubjectDto>>>(parameters)
{
    public override async Task<RequestResult<List<SimpleSubjectDto>>> Handle(GetAllActiveSubjectsQuery request, CancellationToken cancellationToken)
    {
        var subjects = await subjectsRepository.Get(x => x.IsActive)
            .AsNoTracking()
            .Select(s => new SimpleSubjectDto
            {
                Id = s.ID,
                Name = s.Name,
                Icon = s.Icon
            })
            .ToListAsync(cancellationToken);

        return RequestResult<List<SimpleSubjectDto>>.Success(subjects);
    }
}
