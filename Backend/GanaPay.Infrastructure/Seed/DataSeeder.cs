using GanaPay.Core.Entities;
using GanaPay.Core.Enums;
using GanaPay.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GanaPay.Infrastructure.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Asegurar que la base de datos existe
        await context.Database.MigrateAsync();

        // Si ya hay usuarios, no hacer nada
        if (await context.Usuarios.AnyAsync())
        {
            Console.WriteLine("[SEED] ⚠️  La base de datos ya tiene datos. Omitiendo seed.");
            return;
        }

        Console.WriteLine("[SEED] 🌱 Iniciando seed de datos...");

        // ==================== CREAR USUARIOS ====================
        var usuarios = new List<Usuario>
        {
            new Usuario
            {
                NombreCompleto = "Juan Pérez",
                Email = "juan@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                NumeroDocumento = "12345678",
                Telefono = "78901234",
                Rol = RolUsuario.Cliente,
                Activo = true,
                FechaRegistro = DateTime.UtcNow
            },
            new Usuario
            {
                NombreCompleto = "María García",
                Email = "maria@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                NumeroDocumento = "87654321",
                Telefono = "79012345",
                Rol = RolUsuario.Cliente,
                Activo = true,
                FechaRegistro = DateTime.UtcNow
            },
            new Usuario
            {
                NombreCompleto = "Admin Sistema",
                Email = "admin@ganapay.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                NumeroDocumento = "99999999",
                Telefono = "70000000",
                Rol = RolUsuario.Administrador,
                Activo = true,
                FechaRegistro = DateTime.UtcNow
            }
        };

        await context.Usuarios.AddRangeAsync(usuarios);
        await context.SaveChangesAsync();
        Console.WriteLine($"[SEED] ✅ {usuarios.Count} usuarios creados");

        // ==================== CREAR CUENTAS ====================
        var cuentas = new List<Cuenta>
        {
            // Cuentas de Juan (BS y USD)
            new Cuenta
            {
                NumeroCuenta = "100001234567",
                TipoMoneda = TipoMoneda.Bolivianos,
                Saldo = 5000.00m,
                Activa = true,
                FechaCreacion = DateTime.UtcNow,
                UsuarioId = usuarios[0].Id // Juan
            },
            new Cuenta
            {
                NumeroCuenta = "200001234567",
                TipoMoneda = TipoMoneda.Dolares,
                Saldo = 500.00m,
                Activa = true,
                FechaCreacion = DateTime.UtcNow,
                UsuarioId = usuarios[0].Id // Juan
            },
            
            // Cuentas de María (BS y USD)
            new Cuenta
            {
                NumeroCuenta = "100007654321",
                TipoMoneda = TipoMoneda.Bolivianos,
                Saldo = 3000.00m,
                Activa = true,
                FechaCreacion = DateTime.UtcNow,
                UsuarioId = usuarios[1].Id // María
            },
            new Cuenta
            {
                NumeroCuenta = "200007654321",
                TipoMoneda = TipoMoneda.Dolares,
                Saldo = 200.00m,
                Activa = true,
                FechaCreacion = DateTime.UtcNow,
                UsuarioId = usuarios[1].Id // María
            },
            
            // Cuenta del Admin
            new Cuenta
            {
                NumeroCuenta = "100009999999",
                TipoMoneda = TipoMoneda.Bolivianos,
                Saldo = 10000.00m,
                Activa = true,
                FechaCreacion = DateTime.UtcNow,
                UsuarioId = usuarios[2].Id // Admin
            }
        };

        await context.Cuentas.AddRangeAsync(cuentas);
        await context.SaveChangesAsync();
        Console.WriteLine($"[SEED] ✅ {cuentas.Count} cuentas creadas");

        // ==================== CREAR TRANSACCIONES DE EJEMPLO ====================
        var transacciones = new List<Transaccion>
        {
            // Transferencia de Juan a María
            new Transaccion
            {
                TipoTransaccion = TipoTransaccion.Transferencia,
                Monto = 500.00m,
                Moneda = TipoMoneda.Bolivianos,
                Concepto = "Pago de préstamo",
                Estado = EstadoTransaccion.Completada,
                FechaHora = DateTime.UtcNow.AddDays(-5),
                CuentaOrigenId = cuentas[0].Id, // Juan BS
                CuentaDestinoId = cuentas[2].Id  // María BS
            },
            
            // Depósito en cuenta de Juan
            new Transaccion
            {
                TipoTransaccion = TipoTransaccion.Deposito,
                Monto = 1000.00m,
                Moneda = TipoMoneda.Bolivianos,
                Concepto = "Depósito inicial",
                Estado = EstadoTransaccion.Completada,
                FechaHora = DateTime.UtcNow.AddDays(-10),
                CuentaDestinoId = cuentas[0].Id // Juan BS
            },
            
            // Pago de servicio
            new Transaccion
            {
                TipoTransaccion = TipoTransaccion.PagoServicio,
                Monto = 150.00m,
                Moneda = TipoMoneda.Bolivianos,
                Concepto = "Pago de luz",
                Estado = EstadoTransaccion.Completada,
                FechaHora = DateTime.UtcNow.AddDays(-3),
                CuentaOrigenId = cuentas[0].Id, // Juan BS
                ReferenciaExterna = "LUZ-2025-001"
            }
        };

        await context.Transacciones.AddRangeAsync(transacciones);
        await context.SaveChangesAsync();
        Console.WriteLine($"[SEED] ✅ {transacciones.Count} transacciones creadas");

        Console.WriteLine("[SEED] 🎉 Seed completado exitosamente!");
    }
}