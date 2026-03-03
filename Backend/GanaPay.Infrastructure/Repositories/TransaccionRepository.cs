using GanaPay.Core.Entities;
using GanaPay.Core.Enums;
using GanaPay.Core.Interfaces.Repositories;
using GanaPay.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GanaPay.Infrastructure.Repositories;

public class TransaccionRepository : BaseRepository<Transaccion>, ITransaccionRepository
{
    public TransaccionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Transaccion>> GetByCuentaOrigenIdAsync(int cuentaId)
    {
        return await _context.Transacciones
            .Include(t => t.CuentaOrigen)
            .Include(t => t.CuentaDestino)
            .Where(t => t.CuentaOrigenId == cuentaId)
            .OrderByDescending(t => t.FechaHora)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaccion>> GetByCuentaDestinoIdAsync(int cuentaId)
    {
        return await _context.Transacciones
            .Include(t => t.CuentaOrigen)
            .Include(t => t.CuentaDestino)
            .Where(t => t.CuentaDestinoId == cuentaId)
            .OrderByDescending(t => t.FechaHora)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaccion>> GetHistorialCuentaAsync(int cuentaId)
    {
        return await _context.Transacciones
            .Include(t => t.CuentaOrigen)
            .Include(t => t.CuentaDestino)
            .Where(t => t.CuentaOrigenId == cuentaId
                     || t.CuentaDestinoId == cuentaId)
            .OrderByDescending(t => t.FechaHora)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaccion>> GetByEstadoAsync(EstadoTransaccion estado)
    {
        return await _context.Transacciones
            .Include(t => t.CuentaOrigen)
            .Include(t => t.CuentaDestino)
            .Where(t => t.Estado == estado)
            .OrderByDescending(t => t.FechaHora)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaccion>> GetByFechaRangoAsync(
        DateTime desde, DateTime hasta)
    {
        return await _context.Transacciones
            .Include(t => t.CuentaOrigen)
            .Include(t => t.CuentaDestino)
            .Where(t => t.FechaHora >= desde && t.FechaHora <= hasta)
            .OrderByDescending(t => t.FechaHora)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalTransaccionesByTipoAsync(
        int cuentaId, TipoTransaccion tipo)
    {
        return await _context.Transacciones
            .Where(t => t.CuentaOrigenId == cuentaId
                     && t.TipoTransaccion == tipo
                     && t.Estado == EstadoTransaccion.Completada)
            .SumAsync(t => t.Monto);
    }
}