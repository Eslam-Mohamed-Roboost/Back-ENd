namespace API.Application.Events;

public class MissionCompletedEvent
{
    public long StudentId { get; set; }
    public long MissionId { get; set; }
    public string MissionTitle { get; set; } = string.Empty;
    public long BadgeId { get; set; }
    public decimal HoursAwarded { get; set; }
    public DateTime CompletedAt { get; set; }
    public TimeSpan CompletionTime { get; set; }
}

