using System.Security.Claims;
using FluentValidation;
using GanaPay.API.Extensions;
using GanaPay.Application.DTOs.Transacciones;
using GanaPay.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GanaPay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransaccionesController : ControllerBase
{
    private readonly ITransaccionService _transaccionService;
    private readonly IValidator<TransferenciaRequestDTO> _transferenciaValidator;
    private readonly ILogger<TransaccionesController> _logger;

    public TransaccionesController(
        ITransaccionService transaccionService,
        IValidator<TransferenciaRequestDTO> transferenciaValidator,
        ILogger<TransaccionesController> logger)
    {
        _transaccionService = transaccionService;
        _transferenciaValidator = transferenciaValidator;
        _logger = logger;
    }

    [HttpPost("transferir")]
    public async Task<IActionResult> Transferir([FromBody] TransferenciaRequestDTO dto)
    {
        // ID del usuario autenticado desde JWT
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        _logger.LogInformation(
            "Transferencia iniciada - Usuario: {UserId}, Origen: {Origen}, Destino: {Destino}, Monto: {Monto}",
            usuarioId, dto.CuentaOrigenId, dto.CuentaDestinoId, dto.Monto);

        // Validar DTO con FluentValidation
        var validationResult = await _transferenciaValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.ToErrorResponse());

        // Ejecutar transferencia
        var result = await _transaccionService.TransferirAsync(dto, usuarioId);

        if (!result.Exito)
        {
            _logger.LogWarning(
                "Transferencia fallida - Usuario: {UserId}, Motivo: {Motivo}",
                usuarioId, result.Mensaje);
            return BadRequest(new { message = result.Mensaje });
        }

        _logger.LogInformation(
            "Transferencia exitosa - Transaccion: {TransId}, Monto: {Monto}",
            result.Transaccion?.Id, dto.Monto);

        return Ok(result);
    }

    [HttpGet("historial/{cuentaId}")]
    public async Task<IActionResult> GetHistorial(int cuentaId)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        _logger.LogInformation(
            "Historial solicitado - Cuenta: {CuentaId}, Usuario: {UserId}",
            cuentaId, usuarioId);

        var historial = await _transaccionService.GetHistorialAsync(cuentaId, usuarioId);

        return Ok(historial);
    }
}