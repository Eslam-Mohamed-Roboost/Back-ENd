namespace API.Application.Features.Admin.DashboardStats.DTOs;

public class DashboardStatsDto
{
    public List<StatsCardDto> Stats { get; set; } = new();
    public int PendingApprovals { get; set; }
}

public class StatsCardDto
{
    public string Title { get; set; } = string.Empty;
    public object Value { get; set; } = 0;
    public string? Breakdown { get; set; }
    public string? Comparison { get; set; }
    public string Icon { get; set; } = string.Empty;
    public string Trend { get; set; } = "neutral";
}
