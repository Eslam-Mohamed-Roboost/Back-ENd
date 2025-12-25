using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClassEntity = API.Domain.Entities.General.Classes;

namespace API.Application.Features.Admin.Classes.Commands;

public record DeleteClassCommand(long Id) : IRequest<RequestResult<bool>>;

public class DeleteClassCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<ClassEntity> classRepository,
    IRepository<Domain.Entities.User> userRepository)
    : RequestHandlerBase<DeleteClassCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(
        DeleteClassCommand request,
        CancellationToken cancellationToken)
    {
        var classEntity = await classRepository.Get(c => c.ID == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (classEntity == null)
        {
            return RequestResult<bool>.Failure(ErrorCode.NotFound, "Class not found");
        }

        // Check if class has students
        var studentsInClass = await userRepository.Get(u => u.ClassID == request.Id)
            .CountAsync(cancellationToken);

        if (studentsInClass > 0)
        {
            return RequestResult<bool>.Failure(
                ErrorCode.ValidationError, 
                $"Cannot delete class with {studentsInClass} students. Please reassign students first.");
        }

        classRepository.Delete(classEntity);
        await classRepository.SaveChangesAsync();

        return RequestResult<bool>.Success(true, "Class deleted successfully");
    }
}

