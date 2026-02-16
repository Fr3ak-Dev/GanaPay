namespace GanaPay.Application.DTOs.Auth;

public class AuthResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public LoginResponseDTO? Data { get; set; }
}