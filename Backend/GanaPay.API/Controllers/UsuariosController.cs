using GanaPay.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GanaPay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuariosController(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    // GET: api/usuarios
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var usuarios = await _usuarioRepository.GetAllAsync();
        return Ok(usuarios);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id);
        
        if (usuario == null)
        {
            return NotFound(new { message = $"Usuario con ID {id} no encontrado" });
        }
        
        return Ok(usuario);
    }

    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(email);
        
        if (usuario == null)
        {
            return NotFound(new { message = $"Usuario con email {email} no encontrado" });
        }
        
        return Ok(usuario);
    }

    [HttpGet("{id}/with-cuentas")]
    public async Task<IActionResult> GetWithCuentas(int id)
    {
        var usuario = await _usuarioRepository.GetWithCuentasAsync(id);
        
        if (usuario == null)
        {
            return NotFound(new { message = $"Usuario con ID {id} no encontrado" });
        }
        
        return Ok(usuario);
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetCount()
    {
        var count = await _usuarioRepository.CountAsync();
        return Ok(new { total = count });
    }
}