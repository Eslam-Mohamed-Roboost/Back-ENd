namespace API.Domain.Entities.Portfolio;

public class PortfolioMapScore : BaseEntity
{
    public long StudentId { get; set; }
    public long SubjectId { get; set; }
    public string Term { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Score { get; set; }
    public DateTime DateTaken { get; set; }
    public int? Percentile { get; set; }
    public int? Growth { get; set; }
    public int? GoalScore { get; set; }
}
