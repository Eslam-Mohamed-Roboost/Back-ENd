using API.Application.Features.Admin.Classes.DTOs;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClassEntity = API.Domain.Entities.General.Classes;

namespace API.Application.Features.Admin.Classes.Queries;

public record GetClassByIdQuery(long Id) : IRequest<RequestResult<ClassDto>>;

public class GetClassByIdQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<ClassEntity> classRepository,
    IRepository<Domain.Entities.User> userRepository)
    : RequestHandlerBase<GetClassByIdQuery, RequestResult<ClassDto>>(parameters)
{
    public override async Task<RequestResult<ClassDto>> Handle(
        GetClassByIdQuery request,
        CancellationToken cancellationToken)
    {
        var classEntity = await classRepository.Get(c => c.ID == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (classEntity == null)
        {
            return RequestResult<ClassDto>.Failure(ErrorCode.NotFound, "Class not found");
        }

        string? teacherName = null;
        if (classEntity.TeacherId.HasValue)
        {
            var teacher = await userRepository.Get(u => u.ID == classEntity.TeacherId.Value)
                .FirstOrDefaultAsync(cancellationToken);
            teacherName = teacher?.Name;
        }

        var classDto = new ClassDto
        {
            Id = classEntity.ID,
            Name = classEntity.Name,
            Grade = classEntity.Grade,
            TeacherId = classEntity.TeacherId,
            TeacherName = teacherName,
            StudentCount = classEntity.StudentCount,
            SubjectIds = new List<long>(),
            CreatedAt = classEntity.CreatedAt
        };

        return RequestResult<ClassDto>.Success(classDto);
    }
}

