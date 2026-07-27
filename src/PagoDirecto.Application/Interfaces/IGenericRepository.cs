using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PagoDirecto.Application.Interfaces;

public interface IGenericRepository<T> where T : class
{
    IQueryable<T> AsQueryable(bool disableTracking = false);
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(object id);
    Task<T?> FirstOrDefaultAsync(System.Linq.Expressions.Expression<System.Func<T, bool>> predicate, bool disableTracking = false);
    Task<bool> AnyAsync(System.Linq.Expressions.Expression<System.Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}

