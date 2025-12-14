using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Helpers.Attributes;

public class LongAsStringConverter : JsonConverter<long>
{
    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            if (long.TryParse(reader.GetString(), out var value))
                return value;
            throw new JsonException("Invalid long string");
        }
        return reader.GetInt64(); // fallback if it's already a number
    }

    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}