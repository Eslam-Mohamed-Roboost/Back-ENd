namespace API.Application.Events;

public class UserLoggedInEvent
{
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTime LoggedInAt { get; set; }
}

