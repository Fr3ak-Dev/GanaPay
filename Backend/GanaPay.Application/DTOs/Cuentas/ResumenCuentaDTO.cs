using GanaPay.Core.Enums;

namespace GanaPay.Application.DTOs.Cuentas;

public class ResumenCuentaDTO
{
    public int Id { get; set; }
    public string NumeroCuenta { get; set; } = string.Empty;
    public TipoMoneda TipoMoneda { get; set; }
    public string TipoMonedaDescripcion { get; set; } = string.Empty;
    public decimal Saldo { get; set; }
    public bool Activa { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public int TotalTransacciones { get; set; }
}