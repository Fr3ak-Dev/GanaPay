namespace GanaPay.Application.DTOs.Admin;

public class EstadisticasAdminDTO
{
    public int UsuariosActivos { get; set; }
    public int TotalUsuarios { get; set; }
    public int CuentasActivas { get; set; }
    public int TotalCuentas { get; set; }
    public int TransaccionesHoy { get; set; }
    public decimal MontoBolivianosHoy { get; set; }
    public decimal MontoDolaresHoy { get; set; }
    public DateTime? UltimaTransaccion { get; set; }
}