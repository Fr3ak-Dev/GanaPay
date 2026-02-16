using GanaPay.Core.Entities;

namespace GanaPay.Core.Interfaces.Repositories;

public interface IUsuarioRepository : IRepository<Usuario>
{
    // Métodos específicos de Usuario
    Task<Usuario?> GetByEmailAsync(string email);
    Task<Usuario?> GetByNumeroDocumentoAsync(string numeroDocumento);
    Task<Usuario?> GetWithCuentasAsync(int id);
    Task<bool> ExistsEmailAsync(string email);
    Task<bool> ExistsNumeroDocumentoAsync(string numeroDocumento);
}