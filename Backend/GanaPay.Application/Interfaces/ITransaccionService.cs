using GanaPay.Application.DTOs.Transacciones;

namespace GanaPay.Application.Interfaces;

public interface ITransaccionService
{
    Task<TransferenciaResponseDTO> TransferirAsync(
        TransferenciaRequestDTO dto,
        int usuarioId); // Viene del JWT, NO del request body

    Task<IEnumerable<TransaccionDTO>> GetHistorialAsync(
        int cuentaId,
        int usuarioId);
}