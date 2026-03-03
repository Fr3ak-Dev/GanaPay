using GanaPay.Core.Interfaces;
using GanaPay.Core.Interfaces.Repositories;
using GanaPay.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

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

    // ==================== STORED PROCEDURES ====================

    public async Task<object?> GetResumenCuentaAsync(int cuentaId)
    {
        // Necesitamos crear un DbSet temporal para este tipo
        var parameter = new SqlParameter("@cuentaId", cuentaId);

        var sql = "EXEC sp_ObtenerResumenCuenta @cuentaId";

        // Usar ExecuteSqlRaw para procedimientos que retornan datos
        var connection = _context.Database.GetDbConnection();
        await _context.Database.OpenConnectionAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.Add(parameter);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new
                {
                    Id = reader.GetInt32(0),
                    NumeroCuenta = reader.GetString(1),
                    TipoMoneda = reader.GetInt32(2),
                    Saldo = reader.GetDecimal(3),
                    Activa = reader.GetBoolean(4),
                    FechaCreacion = reader.GetDateTime(5),
                    NombreUsuario = reader.GetString(6),
                    TotalTransacciones = reader.GetInt32(7),
                    UltimaTransaccion = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8),
                    TotalEnviado = reader.GetDecimal(9),
                    TotalRecibido = reader.GetDecimal(10)
                };
            }

            return null;
        }
        finally
        {
            await _context.Database.CloseConnectionAsync();
        }
    }

    public async Task<object?> GetEstadisticasAdminAsync()
    {
        var sql = "EXEC sp_ObtenerEstadisticasAdmin";

        var connection = _context.Database.GetDbConnection();
        await _context.Database.OpenConnectionAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new
                {
                    UsuariosActivos = reader.GetInt32(0),
                    TotalUsuarios = reader.GetInt32(1),
                    CuentasActivas = reader.GetInt32(2),
                    TotalCuentas = reader.GetInt32(3),
                    TransaccionesHoy = reader.GetInt32(4),
                    MontoBolivianosHoy = reader.GetDecimal(5),
                    MontoDolaresHoy = reader.GetDecimal(6),
                    UltimaTransaccion = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7)
                };
            }

            return null;
        }
        finally
        {
            await _context.Database.CloseConnectionAsync();
        }
    }

    public async Task<IEnumerable<object>> GetHistorialTransaccionesAsync(
        int usuarioId, DateTime desde, DateTime hasta)
    {
        var sql = "EXEC sp_ObtenerHistorialTransacciones @usuarioId, @desde, @hasta";

        var parameters = new[]
        {
        new SqlParameter("@usuarioId", usuarioId),
        new SqlParameter("@desde", desde),
        new SqlParameter("@hasta", hasta)
    };

        var connection = _context.Database.GetDbConnection();
        await _context.Database.OpenConnectionAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.AddRange(parameters);

            using var reader = await command.ExecuteReaderAsync();

            var results = new List<object>();

            while (await reader.ReadAsync())
            {
                results.Add(new
                {
                    Id = reader.GetInt32(0),
                    TipoTransaccion = reader.GetInt32(1),
                    Monto = reader.GetDecimal(2),
                    Moneda = reader.GetInt32(3),
                    Concepto = reader.GetString(4),
                    Estado = reader.GetInt32(5),
                    FechaHora = reader.GetDateTime(6),
                    CuentaOrigenId = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7),
                    NumeroCuentaOrigen = reader.IsDBNull(8) ? null : reader.GetString(8),
                    CuentaDestinoId = reader.IsDBNull(9) ? (int?)null : reader.GetInt32(9),
                    NumeroCuentaDestino = reader.IsDBNull(10) ? null : reader.GetString(10),
                    ReferenciaExterna = reader.IsDBNull(11) ? null : reader.GetString(11),
                    CodigoQR = reader.IsDBNull(12) ? null : reader.GetString(12)
                });
            }

            return results;
        }
        finally
        {
            await _context.Database.CloseConnectionAsync();
        }
    }

    // ==================== DISPOSE ====================

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}