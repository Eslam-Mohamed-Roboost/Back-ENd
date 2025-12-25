using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClassEntity = API.Domain.Entities.General.Classes;

namespace API.Application.Features.Admin.UpdateUser.Commands;

public record UpdateUserCommand(
    long UserId,
    string? Name = null,
    string? Email = null,
    ApplicationRole? Role = null,
    bool? IsActive = null,
    string? PhoneNumber = null,
    long? ClassID = null) : IRequest<RequestResult<bool>>;

public class UpdateUserCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.User> userRepository,
    IRepository<ClassEntity> classRepository)
    : RequestHandlerBase<UpdateUserCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.Get(u => u.ID == request.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            return RequestResult<bool>.Failure(Domain.Enums.ErrorCode.UserNotFound, "User not found");
        }

        // Update fields if provided
        if (!string.IsNullOrEmpty(request.Name))
        {
            user.Name = request.Name;
        }

        if (!string.IsNullOrEmpty(request.Email))
        {
            user.Email = request.Email;
        }

        if (request.Role.HasValue)
        {
            user.Role = request.Role.Value;
        }

        if (request.IsActive.HasValue)
        {
            user.IsActive = request.IsActive.Value;
        }

        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            user.PhoneNumber = request.PhoneNumber;
        }

        // Handle ClassID update
        if (request.ClassID.HasValue)
        {
            // Only students can be assigned to a class
            if (user.Role != ApplicationRole.Student && request.Role != ApplicationRole.Student)
            {
                return RequestResult<bool>.Failure(Domain.Enums.ErrorCode.ValidationError, "Only students can be assigned to a class");
            }

            // Validate class exists
            var classExists = await classRepository.Get(c => c.ID == request.ClassID.Value)
                .AnyAsync(cancellationToken);

            if (!classExists)
            {
                return RequestResult<bool>.Failure(Domain.Enums.ErrorCode.NotFound, $"Class with ID {request.ClassID.Value} does not exist");
            }

            // Update student count if class is changing
            if (user.ClassID.HasValue && user.ClassID.Value != request.ClassID.Value)
            {
                // Decrement old class count
                var oldClass = await classRepository.Get(c => c.ID == user.ClassID.Value)
                    .FirstOrDefaultAsync(cancellationToken);
                if (oldClass != null && oldClass.StudentCount > 0)
                {
                    oldClass.StudentCount--;
                    classRepository.Update(oldClass);
                }

                // Increment new class count
                var newClass = await classRepository.Get(c => c.ID == request.ClassID.Value)
                    .FirstOrDefaultAsync(cancellationToken);
                if (newClass != null)
                {
                    newClass.StudentCount++;
                    classRepository.Update(newClass);
                }
            }
            else if (!user.ClassID.HasValue)
            {
                // First time assignment - increment count
                var newClass = await classRepository.Get(c => c.ID == request.ClassID.Value)
                    .FirstOrDefaultAsync(cancellationToken);
                if (newClass != null)
                {
                    newClass.StudentCount++;
                    classRepository.Update(newClass);
                }
            }

            user.ClassID = request.ClassID.Value;
        }
        else if (request.ClassID == null && user.ClassID.HasValue)
        {
            // Removing class assignment
            var oldClass = await classRepository.Get(c => c.ID == user.ClassID.Value)
                .FirstOrDefaultAsync(cancellationToken);
            if (oldClass != null && oldClass.StudentCount > 0)
            {
                oldClass.StudentCount--;
                classRepository.Update(oldClass);
            }
            user.ClassID = null;
        }

        user.UpdatedAt = DateTime.UtcNow;

        userRepository.Update(user);
        await userRepository.SaveChangesAsync();
        await classRepository.SaveChangesAsync();

        return RequestResult<bool>.Success(true, "User updated successfully");
    }
}

