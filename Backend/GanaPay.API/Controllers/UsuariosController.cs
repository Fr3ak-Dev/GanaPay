using GanaPay.Application.DTOs.Auth;
using GanaPay.Application.DTOs.Cuentas;
using GanaPay.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GanaPay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(
        IUsuarioRepository usuarioRepository,
        ILogger<UsuariosController> logger)
    {
        _usuarioRepository = usuarioRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Obteniendo todos los usuarios");
        
        var usuarios = await _usuarioRepository.GetAllAsync();
        
        // Mapeo manual: Entidad → DTO
        var usuariosDTO = usuarios.Select(u => new UsuarioDTO
        {
            Id = u.Id,
            NombreCompleto = u.NombreCompleto,
            Email = u.Email,
            NumeroDocumento = u.NumeroDocumento,
            Telefono = u.Telefono,
            Rol = u.Rol,
            Activo = u.Activo,
            FechaRegistro = u.FechaRegistro
        }).ToList();
        
        _logger.LogInformation("Retornando {Count} usuarios", usuariosDTO.Count);
        return Ok(usuariosDTO);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("Buscando usuario con ID: {UserId}", id);
        
        var usuario = await _usuarioRepository.GetByIdAsync(id);
        
        if (usuario == null)
        {
            _logger.LogWarning("Usuario con ID {UserId} no encontrado", id);
            return NotFound(new { message = $"Usuario con ID {id} no encontrado" });
        }
        
        var usuarioDTO = new UsuarioDTO
        {
            Id = usuario.Id,
            NombreCompleto = usuario.NombreCompleto,
            Email = usuario.Email,
            NumeroDocumento = usuario.NumeroDocumento,
            Telefono = usuario.Telefono,
            Rol = usuario.Rol,
            Activo = usuario.Activo,
            FechaRegistro = usuario.FechaRegistro
        };
        
        return Ok(usuarioDTO);
    }

    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        _logger.LogInformation("Buscando usuario con email: {Email}", email);
        
        var usuario = await _usuarioRepository.GetByEmailAsync(email);
        
        if (usuario == null)
        {
            _logger.LogWarning("Usuario con email {Email} no encontrado", email);
            return NotFound(new { message = $"Usuario con email {email} no encontrado" });
        }
        
        var usuarioDTO = new UsuarioDTO
        {
            Id = usuario.Id,
            NombreCompleto = usuario.NombreCompleto,
            Email = usuario.Email,
            NumeroDocumento = usuario.NumeroDocumento,
            Telefono = usuario.Telefono,
            Rol = usuario.Rol,
            Activo = usuario.Activo,
            FechaRegistro = usuario.FechaRegistro
        };
        
        return Ok(usuarioDTO);
    }

    [HttpGet("{id}/with-cuentas")]
    public async Task<IActionResult> GetWithCuentas(int id)
    {
        _logger.LogInformation("Buscando usuario con ID {UserId} con sus cuentas", id);
        
        var usuario = await _usuarioRepository.GetWithCuentasAsync(id);
        
        if (usuario == null)
        {
            _logger.LogWarning("Usuario con ID {UserId} no encontrado", id);
            return NotFound(new { message = $"Usuario con ID {id} no encontrado" });
        }
        
        var usuarioConCuentasDTO = new UsuarioConCuentasDTO
        {
            Id = usuario.Id,
            NombreCompleto = usuario.NombreCompleto,
            Email = usuario.Email,
            NumeroDocumento = usuario.NumeroDocumento,
            Telefono = usuario.Telefono,
            Rol = usuario.Rol,
            Activo = usuario.Activo,
            FechaRegistro = usuario.FechaRegistro,
            Cuentas = usuario.Cuentas.Select(c => new CuentaDTO
            {
                Id = c.Id,
                NumeroCuenta = c.NumeroCuenta,
                TipoMoneda = c.TipoMoneda,
                Saldo = c.Saldo,
                Activa = c.Activa,
                FechaCreacion = c.FechaCreacion,
                UsuarioId = c.UsuarioId,
                NombreUsuario = usuario.NombreCompleto
            }).ToList()
        };
        
        _logger.LogInformation("Usuario {UserName} tiene {CuentasCount} cuentas", 
            usuario.NombreCompleto, usuarioConCuentasDTO.Cuentas.Count);
        
        return Ok(usuarioConCuentasDTO);
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetCount()
    {
        var count = await _usuarioRepository.CountAsync();
        
        _logger.LogInformation("Total de usuarios: {Count}", count);
        
        return Ok(new { total = count });
    }
}