using System.Text.Json.Serialization;
using API.Helpers.Attributes;

namespace API.Application.Features.Admin.Classes.DTOs;

public class ClassDropdownDto
{    
    [JsonConverter(typeof(LongAsStringConverter))]

    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Grade { get; set; }
    public string DisplayName => $"{Name} (Grade {Grade})";
}

