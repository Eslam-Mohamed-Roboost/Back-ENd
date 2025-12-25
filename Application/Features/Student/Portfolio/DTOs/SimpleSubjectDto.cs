
using System.Text.Json.Serialization;
using API.Helpers.Attributes;

namespace API.Application.Features.Student.Portfolio.DTOs;

public class SimpleSubjectDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
}
