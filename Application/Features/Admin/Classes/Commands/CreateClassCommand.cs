using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClassEntity = API.Domain.Entities.General.Classes;

namespace API.Application.Features.Admin.Classes.Commands;

public record CreateClassCommand(
    string Name,
    int Grade,
    long? TeacherId) : IRequest<RequestResult<long>>;

public class CreateClassCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<ClassEntity> classRepository,
    IRepository<Domain.Entities.User> userRepository)
    : RequestHandlerBase<CreateClassCommand, RequestResult<long>>(parameters)
{
    public override async Task<RequestResult<long>> Handle(
        CreateClassCommand request,
        CancellationToken cancellationToken)
    {
        // Validate teacher exists if provided
        if (request.TeacherId.HasValue)
        {
            var teacher = await userRepository.Get(u => u.ID == request.TeacherId.Value)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (teacher == null)
            {
                return RequestResult<long>.Failure(ErrorCode.NotFound, "Teacher not found");
            }

            if (teacher.Role != ApplicationRole.Teacher)
            {
                return RequestResult<long>.Failure(ErrorCode.ValidationError, "User is not a teacher");
            }
        }

        // Check if class name already exists for this grade
        var existingClass = await classRepository.Get(c => c.Name == request.Name && c.Grade == request.Grade)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingClass != null)
        {
            return RequestResult<long>.Failure(ErrorCode.ValidationError, "Class with this name already exists for this grade");
        }

        var newClass = new ClassEntity
        {
            Name = request.Name,
            Grade = request.Grade,
            TeacherId = request.TeacherId,
            StudentCount = 0,
            CreatedAt = DateTime.UtcNow
        };

        var classId = classRepository.Add(newClass);
        await classRepository.SaveChangesAsync();

        return RequestResult<long>.Success(classId, "Class created successfully");
    }
}

