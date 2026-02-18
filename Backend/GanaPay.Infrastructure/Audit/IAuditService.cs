using GanaPay.Infrastructure.Data.Models;

namespace GanaPay.Infrastructure.Audit;

public interface IAuditService
{
    Task LogAsync(AuditLog log);

    Task LogLoginAsync(int usuarioId, string email, string? ip, bool exitoso);

    Task LogTransferenciaAsync(
        int usuarioId,
        string email,
        string cuentaOrigen,
        string cuentaDestino,
        decimal monto,
        string moneda,
        bool exitoso,
        string? motivo = null);

    Task<IEnumerable<AuditLog>> GetByUsuarioIdAsync(int usuarioId, int limit = 50);
    Task<IEnumerable<AuditLog>> GetByTipoAsync(string tipo, int limit = 50);
    Task<IEnumerable<AuditLog>> GetRecientesAsync(int limit = 100);
}