using GanaPay.Core.Enums;

namespace GanaPay.Core.Entities;

public class Transaccion
{
    public int Id { get; set; }
    public TipoTransaccion TipoTransaccion { get; set; }
    public decimal Monto { get; set; }
    public TipoMoneda Moneda { get; set; }
    public string Concepto { get; set; } = string.Empty;
    public EstadoTransaccion Estado { get; set; } = EstadoTransaccion.Pendiente;
    public DateTime FechaHora { get; set; } = DateTime.UtcNow;
    public int? CuentaOrigenId { get; set; } // Nullable porque un depósito no tiene origen
    public int? CuentaDestinoId { get; set; } // Nullable porque un retiro no tiene destino
    
    // Navigation Properties
    public virtual Cuenta? CuentaOrigen { get; set; }
    public virtual Cuenta? CuentaDestino { get; set; }
    
    // Datos adicionales según el tipo
    public string? ReferenciaExterna { get; set; } // Para pagos de servicios
    public string? CodigoQR { get; set; } // Para pagos QR
}