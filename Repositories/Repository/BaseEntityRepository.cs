using DAL.Context;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository;

public class BaseEntityRepository<T> : IBaseEntityRepository<T> where T : class
{
    private readonly ApplicationDbContext _dbContext;

    public BaseEntityRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IQueryable<T> GetAll(Expression<Func<T, bool>>? filter = null, params string[] includes)
    {
        IQueryable<T> query = _dbContext.Set<T>().AsQueryable();

        foreach (string include in includes)
            query = query.Include(include);

        if(filter is not null)
            query = query.Where(filter);
        
        return query;
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> filter) => await _dbContext.Set<T>().FirstOrDefaultAsync(filter);

    public async Task AddAsync(T entity)
    {
        _dbContext.Set<T>().Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        _dbContext.Set<T>().AddRange(entities);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter) => await _dbContext.Set<T>().AnyAsync(filter);

    public async Task RemoveAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbContext.Update<T>(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveRangeAsync(IEnumerable<T> entities)
    {
        _dbContext.Set<T>().RemoveRange(entities);
        await _dbContext.SaveChangesAsync();
    }
}
