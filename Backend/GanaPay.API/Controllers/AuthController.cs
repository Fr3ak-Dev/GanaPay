using FluentValidation;
using GanaPay.Application.DTOs.Auth;
using GanaPay.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GanaPay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<RegisterRequestDTO> _registerValidator;
    private readonly IValidator<LoginRequestDTO> _loginValidator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        IValidator<RegisterRequestDTO> registerValidator,
        IValidator<LoginRequestDTO> loginValidator,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO dto)
    {
        _logger.LogInformation("Intento de registro: {Email}", dto.Email);

        // Validar DTO con FluentValidation
        var validationResult = await _registerValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Registro fallido por validación: {Email}", dto.Email);
            return BadRequest(new
            {
                errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    )
            });
        }

        var result = await _authService.RegisterAsync(dto);

        if (!result.Success)
        {
            _logger.LogWarning("Registro fallido: {Message}", result.Message);
            return BadRequest(new { message = result.Message });
        }

        _logger.LogInformation("Usuario registrado exitosamente: {Email}", dto.Email);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO dto)
    {
        _logger.LogInformation("Intento de login: {Email}", dto.Email);

        // Validar DTO con FluentValidation
        var validationResult = await _loginValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Login fallido por validación: {Email}", dto.Email);
            return BadRequest(new
            {
                errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    )
            });
        }

        var result = await _authService.LoginAsync(dto);

        if (!result.Success)
        {
            _logger.LogWarning("Login fallido: {Email} - {Message}", dto.Email, result.Message);
            return Unauthorized(new { message = result.Message });
        }

        _logger.LogInformation("Login exitoso: {Email}", dto.Email);
        return Ok(result);
    }
}