namespace GanaPay.Application.DTOs.Transacciones;

public class TransferenciaResponseDTO
{
    public bool Exito { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public TransaccionDTO? Transaccion { get; set; }
    public decimal NuevoSaldoOrigen { get; set; }
    public decimal NuevoSaldoDestino { get; set; }
}