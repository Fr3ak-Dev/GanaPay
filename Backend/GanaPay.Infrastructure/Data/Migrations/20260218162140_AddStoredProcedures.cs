using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GanaPay.Infrastructure.Migrations
{
    public partial class AddStoredProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_ObtenerHistorialTransacciones
                    @usuarioId INT,
                    @desde DATETIME2,
                    @hasta DATETIME2
                AS
                BEGIN
                    SET NOCOUNT ON;
                    DECLARE @desdeDate DATE = CAST(@desde AS DATE)
                    DECLARE @hastaDate DATE = CAST(@hasta AS DATE)
                    DECLARE @rangoInicio DATETIME2 = @desdeDate
                    DECLARE @rangoFin DATETIME2 = DATEADD(DAY, 1, @hastaDate)
                    SELECT 
                        t.Id, t.TipoTransaccion, t.Monto, t.Moneda, t.Concepto, t.Estado, t.FechaHora,
                        t.CuentaOrigenId, co.NumeroCuenta AS NumeroCuentaOrigen,
                        t.CuentaDestinoId, cd.NumeroCuenta AS NumeroCuentaDestino,
                        t.ReferenciaExterna, t.CodigoQR
                    FROM Transacciones t
                    LEFT JOIN Cuentas co ON t.CuentaOrigenId = co.Id
                    LEFT JOIN Cuentas cd ON t.CuentaDestinoId = cd.Id
                    WHERE (co.UsuarioId = @usuarioId OR cd.UsuarioId = @usuarioId)
                        AND t.FechaHora >= @rangoInicio AND t.FechaHora < @rangoFin
                    ORDER BY t.FechaHora DESC;
                END
            ");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_ObtenerResumenCuenta
                    @cuentaId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT 
                        c.Id, c.NumeroCuenta, c.TipoMoneda, c.Saldo, c.Activa, c.FechaCreacion,
                        u.NombreCompleto AS NombreUsuario,
                        (SELECT COUNT(*) FROM Transacciones WHERE CuentaOrigenId = c.Id OR CuentaDestinoId = c.Id) AS TotalTransacciones,
                        (SELECT TOP 1 FechaHora FROM Transacciones WHERE CuentaOrigenId = c.Id OR CuentaDestinoId = c.Id ORDER BY FechaHora DESC) AS UltimaTransaccion,
                        ISNULL((SELECT SUM(Monto) FROM Transacciones WHERE CuentaOrigenId = c.Id AND Estado = 2), 0) AS TotalEnviado,
                        ISNULL((SELECT SUM(Monto) FROM Transacciones WHERE CuentaDestinoId = c.Id AND Estado = 2), 0) AS TotalRecibido
                    FROM Cuentas c
                    INNER JOIN Usuarios u ON c.UsuarioId = u.Id
                    WHERE c.Id = @cuentaId;
                END
            ");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_ObtenerEstadisticasAdmin
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT 
                        (SELECT COUNT(*) FROM Usuarios WHERE Activo = 1) AS UsuariosActivos,
                        (SELECT COUNT(*) FROM Usuarios) AS TotalUsuarios,
                        (SELECT COUNT(*) FROM Cuentas WHERE Activa = 1) AS CuentasActivas,
                        (SELECT COUNT(*) FROM Cuentas) AS TotalCuentas,
                        (SELECT COUNT(*) FROM Transacciones WHERE CAST(FechaHora AS DATE) = CAST(GETUTCDATE() AS DATE)) AS TransaccionesHoy,
                        ISNULL((SELECT SUM(Monto) FROM Transacciones WHERE CAST(FechaHora AS DATE) = CAST(GETUTCDATE() AS DATE) AND Moneda = 1 AND Estado = 2), 0) AS MontoBolivianosHoy,
                        ISNULL((SELECT SUM(Monto) FROM Transacciones WHERE CAST(FechaHora AS DATE) = CAST(GETUTCDATE() AS DATE) AND Moneda = 2 AND Estado = 2), 0) AS MontoDolaresHoy,
                        (SELECT TOP 1 FechaHora FROM Transacciones ORDER BY FechaHora DESC) AS UltimaTransaccion;
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ObtenerHistorialTransacciones");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ObtenerResumenCuenta");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ObtenerEstadisticasAdmin");
        }
    }
}