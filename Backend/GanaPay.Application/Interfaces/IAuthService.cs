using GanaPay.Application.DTOs.Auth;

namespace GanaPay.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequestDTO dto);
    Task<AuthResult> LoginAsync(LoginRequestDTO dto);
}