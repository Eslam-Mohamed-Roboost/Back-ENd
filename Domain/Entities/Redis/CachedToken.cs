namespace API.Domain.Entities.Redis;

public class CachedToken
{
    public long ID { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ExpireAt { get; set; }
    public DateTime? LoggedOutAt { get; set; }
    public DateTime? TTL { get; set; }
}

