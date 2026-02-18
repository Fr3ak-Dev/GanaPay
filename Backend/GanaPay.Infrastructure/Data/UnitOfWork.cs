using GanaPay.Core.Interfaces;
using GanaPay.Core.Interfaces.Repositories;
using GanaPay.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace GanaPay.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    // Lazy initialization
    private IUsuarioRepository? _usuarios;
    private ICuentaRepository? _cuentas;
    private ITransaccionRepository? _transacciones;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    // ==================== REPOSITORIOS ====================

    public IUsuarioRepository Usuarios =>
        _usuarios ??= new UsuarioRepository(_context);

    public ICuentaRepository Cuentas =>
        _cuentas ??= new CuentaRepository(_context);

    public ITransaccionRepository Transacciones =>
        _transacciones ??= new TransaccionRepository(_context);

    // ==================== TRANSACCIONES ====================

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
        Console.WriteLine("[UoW] 🟡 Transacción iniciada");
    }

    public async Task<int> CommitAsync()
    {
        try
        {
            var result = await _context.SaveChangesAsync();

            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                Console.WriteLine("[UoW] ✅ Commit exitoso");
            }

            return result; // Número de filas afectadas
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            Console.WriteLine("[UoW] 🔴 Rollback ejecutado");
        }
    }

    // ==================== DISPOSE ====================

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}