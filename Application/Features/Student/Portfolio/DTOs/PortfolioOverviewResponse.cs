namespace API.Application.Features.Student.Portfolio.DTOs;

public class PortfolioOverviewResponse
{
    public int TotalFiles { get; set; }
    public int TotalFeedback { get; set; }
    public int TotalBadges { get; set; }
    public List<SubjectPortfolioDto> SubjectPortfolios { get; set; } = new();
    public List<PortfolioFileDto> RecentUploads { get; set; } = new();
}
