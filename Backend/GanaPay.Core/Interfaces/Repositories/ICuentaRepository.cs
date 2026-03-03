using GanaPay.Core.Entities;

namespace GanaPay.Core.Interfaces.Repositories;

public interface ICuentaRepository : IRepository<Cuenta>
{
    Task<Cuenta?> GetByNumeroCuentaAsync(string numeroCuenta);
    Task<IEnumerable<Cuenta>> GetByUsuarioIdAsync(int usuarioId);
    Task<Cuenta?> GetWithTransaccionesAsync(int id);
    Task<bool> ExistsNumeroCuentaAsync(string numeroCuenta);
    Task<bool> TieneSaldoSuficienteAsync(int cuentaId, decimal monto);
}