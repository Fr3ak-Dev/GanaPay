using GanaPay.Core.Enums;

namespace GanaPay.Core.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty; // CI o NIT
    public string Telefono { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; } = RolUsuario.Cliente;
    public bool Activo { get; set; } = true;
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    // Relaciones
    public virtual ICollection<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
}