namespace API.EndPoints.Admin.MissionsGet;

public class GetMissionsRequest : PageingRequest
{
    public long? badgeId { get; set; }
}

