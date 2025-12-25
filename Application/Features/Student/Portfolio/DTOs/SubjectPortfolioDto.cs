using API.Helpers.Attributes;
using System.Text.Json.Serialization;

namespace API.Application.Features.Student.Portfolio.DTOs;

public class SubjectPortfolioDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string SubjectIcon { get; set; } = string.Empty;
    public List<PortfolioFileDto> Files { get; set; } = new();
    public List<TeacherFeedbackDto> Feedback { get; set; } = new();
    public List<ReflectionDto> Reflections { get; set; } = new();
    public List<PortfolioBadgeDto> Badges { get; set; } = new();
    public PortfolioStatsDto Stats { get; set; } = new();
}
