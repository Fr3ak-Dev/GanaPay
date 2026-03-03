using AutoMapper;
using GanaPay.Application.DTOs.Transacciones;
using GanaPay.Application.Interfaces;
using GanaPay.Core.Entities;
using GanaPay.Core.Enums;
using GanaPay.Core.Interfaces;

namespace GanaPay.Application.Services;

public class TransaccionService : ITransaccionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TransaccionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TransferenciaResponseDTO> TransferirAsync(
        TransferenciaRequestDTO dto,
        int usuarioId)
    {
        // ===== VALIDACIONES DE NEGOCIO =====

        var cuentaOrigen = await _unitOfWork.Cuentas.GetByIdAsync(dto.CuentaOrigenId);

        if (cuentaOrigen == null)
            return Error("La cuenta de origen no existe");

        if (cuentaOrigen.UsuarioId != usuarioId)
            return Error("No tienes permiso para usar esta cuenta");

        if (!cuentaOrigen.Activa)
            return Error("La cuenta de origen está inactiva");

        var cuentaDestino = await _unitOfWork.Cuentas.GetByIdAsync(dto.CuentaDestinoId);

        if (cuentaDestino == null)
            return Error("La cuenta de destino no existe");

        if (!cuentaDestino.Activa)
            return Error("La cuenta de destino está inactiva");

        if (cuentaOrigen.Id == cuentaDestino.Id)
            return Error("No puedes transferir a la misma cuenta");

        if (cuentaOrigen.TipoMoneda != cuentaDestino.TipoMoneda)
            return Error("Las cuentas deben ser de la misma moneda");

        if (dto.Monto <= 0)
            return Error("El monto debe ser mayor a cero");

        if (cuentaOrigen.Saldo < dto.Monto)
            return Error($"Saldo insuficiente. Saldo disponible: {cuentaOrigen.Saldo:F2}");

        // ===== EJECUTAR TRANSFERENCIA CON UNIT OF WORK =====
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Débitar cuenta origen
            cuentaOrigen.Saldo -= dto.Monto;
            await _unitOfWork.Cuentas.UpdateAsync(cuentaOrigen);

            // Acreditar cuenta destino
            cuentaDestino.Saldo += dto.Monto;
            await _unitOfWork.Cuentas.UpdateAsync(cuentaDestino);

            // Registrar transacción
            var transaccion = new Transaccion
            {
                TipoTransaccion = TipoTransaccion.Transferencia,
                Monto = dto.Monto,
                Moneda = cuentaOrigen.TipoMoneda,
                Concepto = dto.Concepto,
                Estado = EstadoTransaccion.Completada,
                FechaHora = DateTime.UtcNow,
                CuentaOrigenId = cuentaOrigen.Id,
                CuentaDestinoId = cuentaDestino.Id
            };

            await _unitOfWork.Transacciones.AddAsync(transaccion);
            await _unitOfWork.CommitAsync();

            // ===== PREPARAR RESPUESTA =====
            var transaccionDTO = _mapper.Map<TransaccionDTO>(transaccion);
            transaccionDTO.NumeroCuentaOrigen = cuentaOrigen.NumeroCuenta;
            transaccionDTO.NumeroCuentaDestino = cuentaDestino.NumeroCuenta;

            return new TransferenciaResponseDTO
            {
                Exito = true,
                Mensaje = "Transferencia realizada con éxito",
                Transaccion = transaccionDTO,
                NuevoSaldoOrigen = cuentaOrigen.Saldo,
                NuevoSaldoDestino = cuentaDestino.Saldo
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();

            return Error($"Error al procesar la transferencia: {ex.Message}");
        }
    }

    public async Task<IEnumerable<TransaccionDTO>> GetHistorialAsync(
        int cuentaId,
        int usuarioId)
    {
        // Verificar que la cuenta pertenece al usuario
        var cuenta = await _unitOfWork.Cuentas.GetByIdAsync(cuentaId);

        if (cuenta == null || cuenta.UsuarioId != usuarioId)
            return Enumerable.Empty<TransaccionDTO>();

        // Obtener historial
        var transacciones = await _unitOfWork.Transacciones
            .GetHistorialCuentaAsync(cuentaId);

        return _mapper.Map<IEnumerable<TransaccionDTO>>(transacciones);
    }

    // ===== MÉTODO HELPER PRIVADO =====
    private static TransferenciaResponseDTO Error(string mensaje)
    {
        return new TransferenciaResponseDTO
        {
            Exito = false,
            Mensaje = mensaje,
            Transaccion = null,
            NuevoSaldoOrigen = 0,
            NuevoSaldoDestino = 0
        };
    }
}