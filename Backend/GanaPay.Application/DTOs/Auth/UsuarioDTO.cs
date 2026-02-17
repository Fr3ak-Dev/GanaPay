using GanaPay.Core.Enums;

namespace GanaPay.Application.DTOs.Auth;

public class UsuarioDTO
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }
}