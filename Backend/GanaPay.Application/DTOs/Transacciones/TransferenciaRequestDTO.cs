namespace GanaPay.Application.DTOs.Transacciones;

public class TransferenciaRequestDTO
{
    public int CuentaOrigenId { get; set; }
    public int CuentaDestinoId { get; set; }
    public decimal Monto { get; set; }
    public string Concepto { get; set; } = string.Empty;
}