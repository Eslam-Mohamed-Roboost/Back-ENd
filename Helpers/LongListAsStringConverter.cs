using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Helpers.Attributes;

public class LongListAsStringConverter : JsonConverter<List<long>>
{
    public override List<long> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null!;

        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Expected start of array");

        var list = new List<long>();
        
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                return list;

            if (reader.TokenType == JsonTokenType.String)
            {
                if (long.TryParse(reader.GetString(), out var value))
                    list.Add(value);
                else
                    throw new JsonException($"Invalid long string: {reader.GetString()}");
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                list.Add(reader.GetInt64());
            }
            else
            {
                throw new JsonException($"Unexpected token type: {reader.TokenType}");
            }
        }

        throw new JsonException("Unexpected end of array");
    }

    public override void Write(Utf8JsonWriter writer, List<long> value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartArray();
        foreach (var item in value)
        {
            writer.WriteStringValue(item.ToString());
        }
        writer.WriteEndArray();
    }
}

