using API.Application.Features.Admin.Classes.DTOs;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClassEntity = API.Domain.Entities.General.Classes;

namespace API.Application.Features.Admin.Classes.Queries;

public record GetClassesQuery : IRequest<RequestResult<List<ClassDto>>>;

public class GetClassesQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<ClassEntity> classRepository,
    IRepository<Domain.Entities.User> userRepository)
    : RequestHandlerBase<GetClassesQuery, RequestResult<List<ClassDto>>>(parameters)
{
    public override async Task<RequestResult<List<ClassDto>>> Handle(
        GetClassesQuery request,
        CancellationToken cancellationToken)
    {
        var classes = await classRepository.Get()
            .OrderBy(c => c.Grade)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);

        var classDtos = new List<ClassDto>();

        foreach (var classEntity in classes)
        {
            string? teacherName = null;
            if (classEntity.TeacherId.HasValue)
            {
                var teacher = await userRepository.Get(u => u.ID == classEntity.TeacherId.Value)
                    .FirstOrDefaultAsync(cancellationToken);
                teacherName = teacher?.Name;
            }

            classDtos.Add(new ClassDto
            {
                Id = classEntity.ID,
                Name = classEntity.Name,
                Grade = classEntity.Grade,
                TeacherId = classEntity.TeacherId,
                TeacherName = teacherName,
                StudentCount = classEntity.StudentCount,
                SubjectIds = new List<long>(), // Will be populated when TeacherClassAssignments is implemented
                CreatedAt = classEntity.CreatedAt
            });
        }

        return RequestResult<List<ClassDto>>.Success(classDtos);
    }
}

