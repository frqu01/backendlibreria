using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PagoDirecto.Application.Interfaces;

public interface IGenericRepository<T> where T : class
{
    IQueryable<T> AsQueryable(bool disableTracking = false);
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(object id);
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    void UpdateRange(IEnumerable<T> entities);
    void RemoveRange(IEnumerable<T> entities);
}

