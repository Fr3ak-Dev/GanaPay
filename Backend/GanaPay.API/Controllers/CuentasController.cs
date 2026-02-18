using System.Security.Claims;
using AutoMapper;
using GanaPay.Application.DTOs.Cuentas;
using GanaPay.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GanaPay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CuentasController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CuentasController> _logger;

    public CuentasController(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CuentasController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet("mis-cuentas")]
    public async Task<IActionResult> GetMisCuentas()
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        _logger.LogInformation("Obteniendo cuentas del usuario {UserId}", usuarioId);

        var cuentas = await _unitOfWork.Cuentas.GetByUsuarioIdAsync(usuarioId);
        var cuentasDTO = _mapper.Map<IEnumerable<CuentaDTO>>(cuentas);

        return Ok(cuentasDTO);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var cuenta = await _unitOfWork.Cuentas.GetByIdAsync(id);

        if (cuenta == null)
            return NotFound(new { message = "Cuenta no encontrada" });

        if (cuenta.UsuarioId != usuarioId)
            return Forbid();

        var cuentaDTO = _mapper.Map<CuentaDTO>(cuenta);
        return Ok(cuentaDTO);
    }

    [HttpGet("numero/{numeroCuenta}")]
    public async Task<IActionResult> GetByNumeroCuenta(string numeroCuenta)
    {
        var cuenta = await _unitOfWork.Cuentas.GetByNumeroCuentaAsync(numeroCuenta);

        if (cuenta == null)
            return NotFound(new { message = "Cuenta no encontrada" });

        // Solo retorna datos básicos (para transferencias)
        // NO retorna saldo (privacidad)
        return Ok(new
        {
            id = cuenta.Id,
            numeroCuenta = cuenta.NumeroCuenta,
            tipoMoneda = cuenta.TipoMoneda,
            nombreUsuario = cuenta.Usuario?.NombreCompleto,
            activa = cuenta.Activa
        });
    }

    [HttpGet("resumen/{id}")]
    public async Task<IActionResult> GetResumen(int id)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        _logger.LogInformation("Resumen de cuenta solicitado - Cuenta: {CuentaId}, Usuario: {UserId}",
            id, usuarioId);

        var cuenta = await _unitOfWork.Cuentas.GetByIdAsync(id);

        if (cuenta == null)
            return NotFound(new { message = "Cuenta no encontrada" });

        if (cuenta.UsuarioId != usuarioId)
            return Forbid();

        // Llamar al SP
        var resumen = await _unitOfWork.GetResumenCuentaAsync(id);

        if (resumen == null)
            return NotFound(new { message = "No se pudo obtener el resumen" });

        return Ok(resumen);
    }

    [HttpGet("historial-sp")]
    public async Task<IActionResult> GetHistorialSP(
        [FromQuery] DateTime? desde,
        [FromQuery] DateTime? hasta)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Si no se especifican fechas, usar el último mes
        var fechaDesde = desde ?? DateTime.UtcNow.AddMonths(-1).Date;   // Date para quitar hora
        var fechaHasta = hasta ?? DateTime.UtcNow.Date;

        _logger.LogInformation(
            "Historial SP solicitado - Usuario: {UserId}, Desde: {Desde:yyyy-MM-dd}, Hasta: {Hasta:yyyy-MM-dd}",
            usuarioId, fechaDesde, fechaHasta);

        var historial = await _unitOfWork.GetHistorialTransaccionesAsync(
            usuarioId, fechaDesde, fechaHasta);

        _logger.LogInformation(
            "Historial SP retornó {Count} registros",
            ((IEnumerable<object>)historial).Count());

        return Ok(historial);
    }
}