using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GanaPay.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStoredProcedures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ==================== SP 1: HISTORIAL DE TRANSACCIONES ====================
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_ObtenerHistorialTransacciones
                    @usuarioId INT,
                    @desde DATETIME2,
                    @hasta DATETIME2
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT 
                        t.Id,
                        t.TipoTransaccion,
                        t.Monto,
                        t.Moneda,
                        t.Concepto,
                        t.Estado,
                        t.FechaHora,
                        t.CuentaOrigenId,
                        co.NumeroCuenta AS NumeroCuentaOrigen,
                        t.CuentaDestinoId,
                        cd.NumeroCuenta AS NumeroCuentaDestino,
                        t.ReferenciaExterna,
                        t.CodigoQR
                    FROM Transacciones t
                    LEFT JOIN Cuentas co ON t.CuentaOrigenId = co.Id
                    LEFT JOIN Cuentas cd ON t.CuentaDestinoId = cd.Id
                    WHERE (co.UsuarioId = @usuarioId OR cd.UsuarioId = @usuarioId)
                        AND t.FechaHora >= @desde
                        AND t.FechaHora <= @hasta
                    ORDER BY t.FechaHora DESC;
                END
            ");

            // ==================== SP 2: RESUMEN DE CUENTA ====================
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_ObtenerResumenCuenta
                    @cuentaId INT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT 
                        c.Id,
                        c.NumeroCuenta,
                        c.TipoMoneda,
                        c.Saldo,
                        c.Activa,
                        c.FechaCreacion,
                        u.NombreCompleto AS NombreUsuario,
                        
                        -- Total de transacciones (como origen + como destino)
                        (
                            SELECT COUNT(*) 
                            FROM Transacciones 
                            WHERE CuentaOrigenId = c.Id OR CuentaDestinoId = c.Id
                        ) AS TotalTransacciones,
                        
                        -- Última transacción
                        (
                            SELECT TOP 1 FechaHora 
                            FROM Transacciones 
                            WHERE CuentaOrigenId = c.Id OR CuentaDestinoId = c.Id
                            ORDER BY FechaHora DESC
                        ) AS UltimaTransaccion,
                        
                        -- Total enviado
                        ISNULL((
                            SELECT SUM(Monto) 
                            FROM Transacciones 
                            WHERE CuentaOrigenId = c.Id 
                                AND Estado = 2  -- Completada
                        ), 0) AS TotalEnviado,
                        
                        -- Total recibido
                        ISNULL((
                            SELECT SUM(Monto) 
                            FROM Transacciones 
                            WHERE CuentaDestinoId = c.Id 
                                AND Estado = 2  -- Completada
                        ), 0) AS TotalRecibido
                        
                    FROM Cuentas c
                    INNER JOIN Usuarios u ON c.UsuarioId = u.Id
                    WHERE c.Id = @cuentaId;
                END
            ");

            // ==================== SP 3: ESTADÍSTICAS ADMIN ====================
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_ObtenerEstadisticasAdmin
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT 
                        -- Usuarios
                        (SELECT COUNT(*) FROM Usuarios WHERE Activo = 1) AS UsuariosActivos,
                        (SELECT COUNT(*) FROM Usuarios) AS TotalUsuarios,
                        
                        -- Cuentas
                        (SELECT COUNT(*) FROM Cuentas WHERE Activa = 1) AS CuentasActivas,
                        (SELECT COUNT(*) FROM Cuentas) AS TotalCuentas,
                        
                        -- Transacciones HOY
                        (
                            SELECT COUNT(*) 
                            FROM Transacciones 
                            WHERE CAST(FechaHora AS DATE) = CAST(GETUTCDATE() AS DATE)
                        ) AS TransaccionesHoy,
                        
                        -- Monto total movido HOY (Bolivianos)
                        ISNULL((
                            SELECT SUM(Monto) 
                            FROM Transacciones 
                            WHERE CAST(FechaHora AS DATE) = CAST(GETUTCDATE() AS DATE)
                                AND Moneda = 1  -- Bolivianos
                                AND Estado = 2  -- Completada
                        ), 0) AS MontoBolivianosHoy,
                        
                        -- Monto total movido HOY (Dólares)
                        ISNULL((
                            SELECT SUM(Monto) 
                            FROM Transacciones 
                            WHERE CAST(FechaHora AS DATE) = CAST(GETUTCDATE() AS DATE)
                                AND Moneda = 2  -- Dolares
                                AND Estado = 2  -- Completada
                        ), 0) AS MontoDolaresHoy,
                        
                        -- Última transacción
                        (
                            SELECT TOP 1 FechaHora 
                            FROM Transacciones 
                            ORDER BY FechaHora DESC
                        ) AS UltimaTransaccion;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ObtenerHistorialTransacciones");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ObtenerResumenCuenta");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ObtenerEstadisticasAdmin");
        }
    }
}