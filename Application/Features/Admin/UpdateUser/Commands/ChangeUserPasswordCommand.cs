using API.Infrastructure.Persistence.Repositories;
using API.Infrastructure.Persistence.DbContexts;
using API.Shared.Helpers;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.UpdateUser.Commands;

public record ChangeUserPasswordCommand(long UserId, string NewPassword) : IRequest<RequestResult<bool>>;

public class ChangeUserPasswordCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.User> userRepository,
    ApplicationDbContext dbContext)
    : RequestHandlerBase<ChangeUserPasswordCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(
        ChangeUserPasswordCommand request,
        CancellationToken cancellationToken)
    {
        // Password validation is handled by FluentValidation in the endpoint
        // Get user
        var user = await userRepository.Get(u => u.ID == request.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            return RequestResult<bool>.Failure(Domain.Enums.ErrorCode.UserNotFound, "User not found");
        }

        // Generate new salt and hash password (same as Register)
        var newSalt = SecurityHelper.GenerateSalt();
        var hashedPassword = SecurityHelper.GetHashedPassword(request.NewPassword, newSalt);

        // Update password and salt (same approach as Register)
        user.Password = hashedPassword;
        user.SaltPassword = newSalt;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = _userState.UserID;

        // Mark entity as modified and save (same pattern as other updates)
        userRepository.Update(user);
        
        // Ensure Password and SaltPassword are explicitly marked as modified
        // This ensures EF Core tracks these changes correctly
        var entry = dbContext.Entry(user);
        if (entry.State == EntityState.Detached)
        {
            dbContext.Attach(user);
            entry = dbContext.Entry(user);
        }
        entry.Property(u => u.Password).IsModified = true;
        entry.Property(u => u.SaltPassword).IsModified = true;
        entry.Property(u => u.UpdatedAt).IsModified = true;
        entry.Property(u => u.UpdatedBy).IsModified = true;
        
        await userRepository.SaveChangesAsync();

        return RequestResult<bool>.Success(true, "Password changed successfully");
    }
}

