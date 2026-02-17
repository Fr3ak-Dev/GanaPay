namespace GanaPay.Application.DTOs.Auth;

public class RegisterRequestDTO
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
}