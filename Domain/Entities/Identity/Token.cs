namespace API.Domain.Entities.Identity;

public class Token : BaseEntity
{
    public long UserID { get; set; }
    public long JwtID { get; set; }
    public DateTime? LoggedOutAt { get; set; }

    public bool IsActive { get; set; } = true;
}

