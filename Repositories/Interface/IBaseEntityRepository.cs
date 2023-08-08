using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface;

public interface IBaseEntityRepository<T> where T : class
{
    IQueryable<T> GetAll(Expression<Func<T, bool>>? filter = null, params string[] includes);

    Task AddAsync(T entity);

    Task AddRangeAsync(IEnumerable<T> entities);

    Task<bool> AnyAsync(Expression<Func<T, bool>> filter);

    Task RemoveAsync(T entity);

    Task RemoveRangeAsync(IEnumerable<T> entities);
}
