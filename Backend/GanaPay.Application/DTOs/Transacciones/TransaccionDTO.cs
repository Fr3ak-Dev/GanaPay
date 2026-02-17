using GanaPay.Core.Enums;

namespace GanaPay.Application.DTOs.Transacciones;

public class TransaccionDTO
{
    public int Id { get; set; }
    public TipoTransaccion TipoTransaccion { get; set; }
    public decimal Monto { get; set; }
    public TipoMoneda Moneda { get; set; }
    public string Concepto { get; set; } = string.Empty;
    public EstadoTransaccion Estado { get; set; }
    public DateTime FechaHora { get; set; }
    
    // Cuenta origen (puede ser null en depósitos)
    public int? CuentaOrigenId { get; set; }
    public string? NumeroCuentaOrigen { get; set; }
    
    // Cuenta destino (puede ser null en retiros)
    public int? CuentaDestinoId { get; set; }
    public string? NumeroCuentaDestino { get; set; }
    
    // Datos adicionales
    public string? ReferenciaExterna { get; set; }
    public string? CodigoQR { get; set; }
}