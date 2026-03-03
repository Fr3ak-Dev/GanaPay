using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GanaPay.Infrastructure.Data.Models;

public class AuditLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("tipo")]
    public string Tipo { get; set; } = string.Empty;

    [BsonElement("usuarioId")]
    public int? UsuarioId { get; set; }

    [BsonElement("email")]
    public string? Email { get; set; }

    [BsonElement("accion")]
    public string Accion { get; set; } = string.Empty;

    [BsonElement("detalles")]
    public BsonDocument? Detalles { get; set; }

    [BsonElement("ip")]
    public string? Ip { get; set; }

    [BsonElement("fechaHora")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime FechaHora { get; set; } = DateTime.UtcNow;

    [BsonElement("exitoso")]
    public bool Exitoso { get; set; }
}