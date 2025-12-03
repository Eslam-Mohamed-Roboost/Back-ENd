using DotNetCore.CAP;
using API.Application.Events;

namespace API.Application.EventHandlers;

public class UserRegisteredEventHandler : ICapSubscribe
{
    private readonly ILogger<UserRegisteredEventHandler> _logger;

    public UserRegisteredEventHandler(ILogger<UserRegisteredEventHandler> logger)
    {
        _logger = logger;
    }

    [CapSubscribe("user.registered", Group = "notification.service")]
    public async Task HandleUserRegistered(UserRegisteredEvent @event)
    {
        _logger.LogInformation(
            "New user registered: {UserName} ({Email}) at {RegisteredAt}",
            @event.UserName, @event.Email, @event.RegisteredAt);

        // TODO: Send welcome email
        // TODO: Create user profile
        // TODO: Add to default groups
        
        await Task.CompletedTask;
    }

    [CapSubscribe("user.registered", Group = "analytics.service")]
    public async Task TrackUserRegistration(UserRegisteredEvent @event)
    {
        _logger.LogInformation(
            "Tracking user registration for analytics: UserId={UserId}",
            @event.UserId);

        // TODO: Send to analytics service
        // TODO: Update metrics
        
        await Task.CompletedTask;
    }
}

