using GanaPay.Core.Entities;
using GanaPay.Core.Interfaces.Repositories;
using GanaPay.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GanaPay.Infrastructure.Repositories;

public class CuentaRepository : BaseRepository<Cuenta>, ICuentaRepository
{
    public CuentaRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Cuenta?> GetByNumeroCuentaAsync(string numeroCuenta)
    {
        return await _context.Cuentas
            .Include(c => c.Usuario)
            .FirstOrDefaultAsync(c => c.NumeroCuenta == numeroCuenta);
    }

    public async Task<IEnumerable<Cuenta>> GetByUsuarioIdAsync(int usuarioId)
    {
        return await _context.Cuentas
            .Where(c => c.UsuarioId == usuarioId && c.Activa)
            .OrderBy(c => c.TipoMoneda)
            .ToListAsync();
    }

    public async Task<Cuenta?> GetWithTransaccionesAsync(int id)
    {
        return await _context.Cuentas
            .Include(c => c.Usuario)
            .Include(c => c.TransaccionesOrigen)
            .Include(c => c.TransaccionesDestino)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<bool> ExistsNumeroCuentaAsync(string numeroCuenta)
    {
        return await _context.Cuentas
            .AnyAsync(c => c.NumeroCuenta == numeroCuenta);
    }

    public async Task<bool> TieneSaldoSuficienteAsync(int cuentaId, decimal monto)
    {
        var cuenta = await _context.Cuentas
            .FirstOrDefaultAsync(c => c.Id == cuentaId);

        return cuenta != null && cuenta.Saldo >= monto;
    }
}