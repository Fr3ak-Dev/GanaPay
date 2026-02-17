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
}