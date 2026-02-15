using GanaPay.Core.Entities;
using GanaPay.Core.Interfaces.Repositories;
using GanaPay.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GanaPay.Infrastructure.Repositories;

public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Usuario?> GetByNumeroDocumentoAsync(string numeroDocumento)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.NumeroDocumento == numeroDocumento);
    }

    public async Task<Usuario?> GetWithCuentasAsync(int id)
    {
        return await _context.Usuarios
            .Include(u => u.Cuentas)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<bool> ExistsEmailAsync(string email)
    {
        return await _context.Usuarios
            .AnyAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsNumeroDocumentoAsync(string numeroDocumento)
    {
        return await _context.Usuarios
            .AnyAsync(u => u.NumeroDocumento == numeroDocumento);
    }
}