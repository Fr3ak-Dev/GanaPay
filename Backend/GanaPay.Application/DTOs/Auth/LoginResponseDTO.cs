namespace GanaPay.Application.DTOs.Auth;

public class LoginResponseDTO
{
    public string Token { get; set; } = string.Empty;
    public UsuarioDTO Usuario { get; set; } = null!;
}