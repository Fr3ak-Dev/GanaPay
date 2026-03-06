using GanaPay.Core.Interfaces.Repositories;

namespace GanaPay.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUsuarioRepository Usuarios { get; }
    ICuentaRepository Cuentas { get; }
    ITransaccionRepository Transacciones { get; }

    Task BeginTransactionAsync();
    Task<int> CommitAsync();
    Task RollbackAsync();

    Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation);

    // ==================== STORED PROCEDURES ====================

    Task<object?> GetResumenCuentaAsync(int cuentaId);  // retorna object genérico, Application/Service hace el cast al DTO correcto
    Task<object?> GetEstadisticasAdminAsync();
    Task<IEnumerable<object>> GetHistorialTransaccionesAsync(
        int usuarioId, DateTime desde, DateTime hasta);
}