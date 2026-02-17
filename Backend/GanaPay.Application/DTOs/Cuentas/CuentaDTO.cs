using GanaPay.Core.Enums;

namespace GanaPay.Application.DTOs.Cuentas;

public class CuentaDTO
{
    public int Id { get; set; }
    public string NumeroCuenta { get; set; } = string.Empty;
    public TipoMoneda TipoMoneda { get; set; }
    public decimal Saldo { get; set; }
    public bool Activa { get; set; }
    public DateTime FechaCreacion { get; set; }
    
    // Datos del usuario (opcional, solo cuando se necesita)
    public int UsuarioId { get; set; }
    public string? NombreUsuario { get; set; }
}