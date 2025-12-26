using System.Text.Json.Serialization;
using API.Helpers.Attributes;

namespace API.Application.Features.Admin.Classes.DTOs;

public class ClassDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Grade { get; set; }
    [JsonConverter(typeof(LongAsStringConverter))]
    public long? TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public int StudentCount { get; set; }
    [JsonConverter(typeof(LongListAsStringConverter))]
    public List<long> SubjectIds { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class CreateClassRequest
{
    public string Name { get; set; } = string.Empty;
    public int Grade { get; set; }
    [JsonConverter(typeof(LongAsStringConverter))]
    public long? TeacherId { get; set; }
}

public class UpdateClassRequest
{
    public string Name { get; set; } = string.Empty;
    public int Grade { get; set; }
    [JsonConverter(typeof(LongAsStringConverter))]
    public long? TeacherId { get; set; }
}

