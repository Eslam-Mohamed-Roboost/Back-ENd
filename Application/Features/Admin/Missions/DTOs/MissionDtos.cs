using System.Text.Json;
using System.Text.Json.Serialization;
using API.Helpers.Attributes;

namespace API.Application.Features.Admin.Missions.DTOs;

public class CreateMissionDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int EstimatedMinutes { get; set; } = 30;
    
    [JsonConverter(typeof(LongAsStringConverter))]
    public long BadgeId { get; set; }
}

public class UpdateMissionDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    
    // Accept both string and int for Duration, convert string to int
    [JsonConverter(typeof(DurationConverter))]
    public int Duration { get; set; } = 30;
    
    [JsonConverter(typeof(LongAsStringConverter))]
    public long BadgeId { get; set; }
    public int Order { get; set; }
    public bool Enabled { get; set; } = true;
}

// Converter to handle Duration as either string or int
public class DurationConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (int.TryParse(stringValue, out var intValue))
            {
                return intValue;
            }
            return 30; // Default if parsing fails
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt32();
        }
        return 30; // Default
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}

public class MissionDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public int Number { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int EstimatedMinutes { get; set; }
    
    [JsonConverter(typeof(LongAsStringConverter))]
    public long BadgeId { get; set; }
    public string? BadgeName { get; set; }
    public int Order { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
}

