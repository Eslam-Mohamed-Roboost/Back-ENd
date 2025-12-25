using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.UpdateUser.Commands;

public record UpdateUserStatusCommand(long UserId, bool IsActive) : IRequest<RequestResult<bool>>;

public class UpdateUserStatusCommandHandler(RequestHandlerBaseParameters parameters, IRepository<Domain.Entities.User> repository)
    : RequestHandlerBase<UpdateUserStatusCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(UpdateUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.Get().FirstOrDefaultAsync(u => u.ID == request.UserId, cancellationToken);
        
        if (user == null)
        {
            return RequestResult<bool>.Failure(Domain.Enums.ErrorCode.UserNotFound, "User not found");
        }

        user.IsActive = request.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        repository.Update(user);
        await repository.SaveChangesAsync();

        return RequestResult<bool>.Success(true, request.IsActive ? "User activated" : "User deactivated");
    }
}
