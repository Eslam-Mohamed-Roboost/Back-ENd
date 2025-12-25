using API.Application.Features.Admin.Classes.DTOs;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClassEntity = API.Domain.Entities.General.Classes;

namespace API.Application.Features.Admin.Classes.Queries;

public record GetClassesDropdownQuery : IRequest<RequestResult<List<ClassDropdownDto>>>;

public class GetClassesDropdownQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<ClassEntity> classRepository)
    : RequestHandlerBase<GetClassesDropdownQuery, RequestResult<List<ClassDropdownDto>>>(parameters)
{
    public override async Task<RequestResult<List<ClassDropdownDto>>> Handle(
        GetClassesDropdownQuery request,
        CancellationToken cancellationToken)
    {
        var classes = await classRepository.Get()
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.Grade)
            .ThenBy(c => c.Name)
            .Select(c => new ClassDropdownDto
            {
                Id = c.ID,
                Name = c.Name,
                Grade = c.Grade
            })
            .ToListAsync(cancellationToken);

        return RequestResult<List<ClassDropdownDto>>.Success(classes);
    }
}

