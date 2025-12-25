using API.Helpers.Attributes;
using System.Text.Json.Serialization;

namespace API.Application.Features.Student.Portfolio.DTOs;

public class PortfolioUploadRequest
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }
    public IFormFile File { get; set; } = null!;
    public string? Description { get; set; }
}

public class SaveReflectionRequest
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Prompt { get; set; }
}
