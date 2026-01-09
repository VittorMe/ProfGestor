using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProfGestor.Converters;

public class CharJsonConverter : JsonConverter<char?>
{
    public override char? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (value.Length == 1)
                return char.ToUpper(value[0]);

            throw new JsonException($"Unable to convert \"{value}\" to char.");
        }

        throw new JsonException($"Unexpected token type: {reader.TokenType}");
    }

    public override void Write(Utf8JsonWriter writer, char? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteStringValue(value.Value.ToString());
        else
            writer.WriteNullValue();
    }
}
