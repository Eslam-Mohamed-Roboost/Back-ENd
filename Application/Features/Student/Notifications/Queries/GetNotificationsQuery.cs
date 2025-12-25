using API.Application.Features.Student.Dashboard.DTOs;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Student.Notifications.Queries;

public record GetNotificationsQuery(bool? UnreadOnly) : IRequest<RequestResult<List<NotificationDto>>>;

public class GetNotificationsQueryHandler(
    RequestHandlerBaseParameters parameters)
    : RequestHandlerBase<GetNotificationsQuery, RequestResult<List<NotificationDto>>>(parameters)
{
    public override async Task<RequestResult<List<NotificationDto>>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // TODO: Implement when Notification entity is created
        // For now, return empty list as placeholder
        var notifications = new List<NotificationDto>();

        // Example placeholder notifications
        if (!request.UnreadOnly.HasValue || !request.UnreadOnly.Value)
        {
            notifications.Add(new NotificationDto
            {
                Id = 1,
                Type = "badge",
                Title = "New Badge Earned!",
                Message = "Congratulations! You've earned a new badge.",
                Date = DateTime.UtcNow.AddDays(-1),
                Read = true,
                ActionUrl = "/student/badges"
            });
        }

        return RequestResult<List<NotificationDto>>.Success(notifications);
    }
}
