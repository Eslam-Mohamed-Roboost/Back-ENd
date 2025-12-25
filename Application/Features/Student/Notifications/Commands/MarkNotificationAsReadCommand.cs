using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Student.Notifications.Commands;

public record MarkNotificationAsReadCommand(long NotificationId) : IRequest<RequestResult<bool>>;

public class MarkNotificationAsReadCommandHandler(
    RequestHandlerBaseParameters parameters)
    : RequestHandlerBase<MarkNotificationAsReadCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // TODO: Implement when Notification entity is created
        // For now, return success as placeholder
        
        return RequestResult<bool>.Success(true);
    }
}
