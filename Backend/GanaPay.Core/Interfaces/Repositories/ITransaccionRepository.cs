using GanaPay.Core.Entities;
using GanaPay.Core.Enums;

namespace GanaPay.Core.Interfaces.Repositories;

public interface ITransaccionRepository : IRepository<Transaccion>
{
    Task<IEnumerable<Transaccion>> GetByCuentaOrigenIdAsync(int cuentaId);
    Task<IEnumerable<Transaccion>> GetByCuentaDestinoIdAsync(int cuentaId);
    Task<IEnumerable<Transaccion>> GetHistorialCuentaAsync(int cuentaId);
    Task<IEnumerable<Transaccion>> GetByEstadoAsync(EstadoTransaccion estado);
    Task<IEnumerable<Transaccion>> GetByFechaRangoAsync(DateTime desde, DateTime hasta);
    Task<decimal> GetTotalTransaccionesByTipoAsync(int cuentaId, TipoTransaccion tipo);
}