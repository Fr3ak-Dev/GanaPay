using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace GanaPay.API.Converters;

public class BsonDocumentJsonConverter : JsonConverter<BsonDocument>
{
    public override BsonDocument Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException("Deserialización no implementada");
    }

    public override void Write(
        Utf8JsonWriter writer,
        BsonDocument value,
        JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var element in value.Elements)
        {
            writer.WritePropertyName(element.Name);
            WriteBsonValue(writer, element.Value);
        }

        writer.WriteEndObject();
    }

    private void WriteBsonValue(Utf8JsonWriter writer, BsonValue value)
    {
        switch (value.BsonType)
        {
            case BsonType.String:
                writer.WriteStringValue(value.AsString);
                break;
            case BsonType.Int32:
                writer.WriteNumberValue(value.AsInt32);
                break;
            case BsonType.Int64:
                writer.WriteNumberValue(value.AsInt64);
                break;
            case BsonType.Double:
                writer.WriteNumberValue(value.AsDouble);
                break;
            case BsonType.Boolean:
                writer.WriteBooleanValue(value.AsBoolean);
                break;
            case BsonType.DateTime:
                writer.WriteStringValue(value.ToUniversalTime());
                break;
            case BsonType.Null:
                writer.WriteNullValue();
                break;
            case BsonType.Document:
                Write(writer, value.AsBsonDocument, null!);
                break;
            case BsonType.Array:
                writer.WriteStartArray();
                foreach (var item in value.AsBsonArray)
                {
                    WriteBsonValue(writer, item);
                }
                writer.WriteEndArray();
                break;
            default:
                writer.WriteStringValue(value.ToString());
                break;
        }
    }
}