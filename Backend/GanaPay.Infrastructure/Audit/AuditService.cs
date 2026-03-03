using GanaPay.Application.Settings;
using GanaPay.Infrastructure.Data.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GanaPay.Infrastructure.Audit;

public class AuditService : IAuditService
{
    private readonly IMongoCollection<AuditLog> _auditLogs;

    public AuditService(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _auditLogs = database.GetCollection<AuditLog>("auditLogs");

        CreateIndexes();
    }

    private void CreateIndexes()
    {
        var indexKeys = Builders<AuditLog>.IndexKeys
            .Descending(log => log.FechaHora);

        var indexModel = new CreateIndexModel<AuditLog>(indexKeys);
        _auditLogs.Indexes.CreateOne(indexModel);
    }

    public async Task LogAsync(AuditLog log)
    {
        try
        {
            await _auditLogs.InsertOneAsync(log);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AUDIT] Error al guardar log: {ex.Message}");
        }
    }

    public async Task LogLoginAsync(
        int usuarioId,
        string email,
        string? ip,
        bool exitoso)
    {
        var log = new AuditLog
        {
            Tipo = "LOGIN",
            UsuarioId = usuarioId,
            Email = email,
            Accion = exitoso ? "Login exitoso" : "Login fallido",
            Ip = ip,
            FechaHora = DateTime.UtcNow,
            Exitoso = exitoso
        };

        await LogAsync(log);
    }

    public async Task LogTransferenciaAsync(
        int usuarioId,
        string email,
        string cuentaOrigen,
        string cuentaDestino,
        decimal monto,
        string moneda,
        bool exitoso,
        string? motivo = null)
    {
        var detalles = new BsonDocument
        {
            { "cuentaOrigen", cuentaOrigen },
            { "cuentaDestino", cuentaDestino },
            { "monto", (double)monto },
            { "moneda", moneda }
        };

        if (!string.IsNullOrEmpty(motivo))
        {
            detalles.Add("motivo", motivo);
        }

        var log = new AuditLog
        {
            Tipo = "TRANSFERENCIA",
            UsuarioId = usuarioId,
            Email = email,
            Accion = exitoso
                ? $"Transferencia de {moneda} {monto:F2}"
                : $"Intento de transferencia fallido: {motivo}",
            Detalles = detalles,
            FechaHora = DateTime.UtcNow,
            Exitoso = exitoso
        };

        await LogAsync(log);
    }

    public async Task<IEnumerable<AuditLog>> GetByUsuarioIdAsync(
        int usuarioId,
        int limit = 50)
    {
        var filter = Builders<AuditLog>.Filter.Eq(log => log.UsuarioId, usuarioId);
        var sort = Builders<AuditLog>.Sort.Descending(log => log.FechaHora);

        return await _auditLogs
            .Find(filter)
            .Sort(sort)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByTipoAsync(
        string tipo,
        int limit = 50)
    {
        var filter = Builders<AuditLog>.Filter.Eq(log => log.Tipo, tipo);
        var sort = Builders<AuditLog>.Sort.Descending(log => log.FechaHora);

        return await _auditLogs
            .Find(filter)
            .Sort(sort)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetRecientesAsync(int limit = 100)
    {
        var sort = Builders<AuditLog>.Sort.Descending(log => log.FechaHora);

        return await _auditLogs
            .Find(_ => true)
            .Sort(sort)
            .Limit(limit)
            .ToListAsync();
    }
}