using AutoMapper;
using GanaPay.Application.DTOs.Auth;
using GanaPay.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GanaPay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(
        IUsuarioRepository usuarioRepository,
        IMapper mapper,
        ILogger<UsuariosController> logger)
    {
        _usuarioRepository = usuarioRepository;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Obteniendo todos los usuarios");
        
        var usuarios = await _usuarioRepository.GetAllAsync();
        
        // AutoMapper: Mapea List<Usuario> → List<UsuarioDTO>
        var usuariosDTO = _mapper.Map<List<UsuarioDTO>>(usuarios);
        
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
        
        var usuarioDTO = _mapper.Map<UsuarioDTO>(usuario);
        
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
        
        var usuarioDTO = _mapper.Map<UsuarioDTO>(usuario);
        
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

        var usuarioConCuentasDTO = _mapper.Map<UsuarioConCuentasDTO>(usuario);
        
        _logger.LogInformation("Usuario {UserName} tiene {CuentasCount} cuentas", 
            usuario.NombreCompleto, usuarioConCuentasDTO.Cuentas.Count);
        
        return Ok(usuarioConCuentasDTO);
    }

    [HttpGet("count")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> GetCount()
    {
        var count = await _usuarioRepository.CountAsync();
        
        _logger.LogInformation("Total de usuarios: {Count}", count);
        
        return Ok(new { total = count });
    }
}