namespace GanaPay.Core.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    // Consultas
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
    
    // Comandos
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    
    // Utilidades
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync();
}