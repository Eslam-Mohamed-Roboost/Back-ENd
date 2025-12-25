using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClassEntity = API.Domain.Entities.General.Classes;

namespace API.Application.Features.Admin.Classes.Commands;

public record UpdateClassCommand(
    long Id,
    string Name,
    int Grade,
    long? TeacherId) : IRequest<RequestResult<bool>>;

public class UpdateClassCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<ClassEntity> classRepository,
    IRepository<Domain.Entities.User> userRepository)
    : RequestHandlerBase<UpdateClassCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(
        UpdateClassCommand request,
        CancellationToken cancellationToken)
    {
        var classEntity = await classRepository.Get(c => c.ID == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (classEntity == null)
        {
            return RequestResult<bool>.Failure(ErrorCode.NotFound, "Class not found");
        }

        // Validate teacher exists if provided
        if (request.TeacherId.HasValue)
        {
            var teacher = await userRepository.Get(u => u.ID == request.TeacherId.Value)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (teacher == null)
            {
                return RequestResult<bool>.Failure(ErrorCode.NotFound, "Teacher not found");
            }

            if (teacher.Role != ApplicationRole.Teacher)
            {
                return RequestResult<bool>.Failure(ErrorCode.ValidationError, "User is not a teacher");
            }
        }

        // Check if new name conflicts with existing class (excluding current class)
        var existingClass = await classRepository.Get(c => 
            c.Name == request.Name && 
            c.Grade == request.Grade && 
            c.ID != request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingClass != null)
        {
            return RequestResult<bool>.Failure(ErrorCode.ValidationError, "Class with this name already exists for this grade");
        }

        classEntity.Name = request.Name;
        classEntity.Grade = request.Grade;
        classEntity.TeacherId = request.TeacherId;
        classEntity.UpdatedAt = DateTime.UtcNow;

        classRepository.Update(classEntity);
        await classRepository.SaveChangesAsync();

        return RequestResult<bool>.Success(true, "Class updated successfully");
    }
}

