using GanaPay.Core.Enums;

namespace GanaPay.Core.Entities;

public class Cuenta
{
    public int Id { get; set; }
    public string NumeroCuenta { get; set; } = string.Empty;
    public TipoMoneda TipoMoneda { get; set; }
    public decimal Saldo { get; set; } = 0;
    public bool Activa { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public int UsuarioId { get; set; }
    
    // Navigation Property
    public virtual Usuario Usuario { get; set; } = null!;
    
    // Relaciones
    public virtual ICollection<Transaccion> TransaccionesOrigen { get; set; } = new List<Transaccion>();
    public virtual ICollection<Transaccion> TransaccionesDestino { get; set; } = new List<Transaccion>();
}