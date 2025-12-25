namespace API.Application.Events;

public class ChallengeCompletedEvent
{
    public long UserId { get; set; }
    public bool IsTeacher { get; set; }
    public long ChallengeId { get; set; }
    public string ChallengeTitle { get; set; } = string.Empty;
    public long? BadgeId { get; set; }
    public decimal HoursAwarded { get; set; }
    public DateTime CompletedAt { get; set; }
}

