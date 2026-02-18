using GanaPay.Core.Enums;

namespace GanaPay.Application.DTOs.Cuentas;

public class CrearCuentaDTO
{
    public int UsuarioId { get; set; }
    public TipoMoneda TipoMoneda { get; set; }
    public decimal SaldoInicial { get; set; }
}