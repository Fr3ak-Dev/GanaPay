using GanaPay.Application.DTOs.Admin;
using GanaPay.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GanaPay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class AdminController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IUnitOfWork unitOfWork,
        ILogger<AdminController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet("estadisticas")]
    public async Task<IActionResult> GetEstadisticas()
    {
        _logger.LogInformation("Estadísticas solicitadas por admin");

        var estadisticas = await _unitOfWork.GetEstadisticasAdminAsync();

        if (estadisticas == null)
            return NotFound(new { message = "No se pudieron obtener estadísticas" });

        return Ok(estadisticas);
    }
}