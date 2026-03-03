using GanaPay.Infrastructure.Audit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GanaPay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;
    private readonly ILogger<AuditController> _logger;

    public AuditController(
        IAuditService auditService,
        ILogger<AuditController> logger)
    {
        _auditService = auditService;
        _logger = logger;
    }

    [HttpGet("recientes")]
    public async Task<IActionResult> GetRecientes([FromQuery] int limit = 100)
    {
        _logger.LogInformation("Logs de auditoría recientes solicitados (limit: {Limit})", limit);

        if (limit > 500)
            return BadRequest(new { message = "El límite máximo es 500 registros" });

        var logs = await _auditService.GetRecientesAsync(limit);
        return Ok(logs);
    }

    [HttpGet("usuario/{usuarioId}")]
    public async Task<IActionResult> GetByUsuarioId(int usuarioId, [FromQuery] int limit = 50)
    {
        _logger.LogInformation(
            "Logs de auditoría solicitados para usuario {UserId} (limit: {Limit})",
            usuarioId, limit);

        if (limit > 200)
            return BadRequest(new { message = "El límite máximo es 200 registros" });

        var logs = await _auditService.GetByUsuarioIdAsync(usuarioId, limit);
        return Ok(logs);
    }

    [HttpGet("tipo/{tipo}")]
    public async Task<IActionResult> GetByTipo(string tipo, [FromQuery] int limit = 50)
    {
        _logger.LogInformation(
            "Logs de auditoría solicitados para tipo {Tipo} (limit: {Limit})",
            tipo, limit);

        if (limit > 200)
            return BadRequest(new { message = "El límite máximo es 200 registros" });

        var logs = await _auditService.GetByTipoAsync(tipo.ToUpper(), limit);
        return Ok(logs);
    }
}