using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Repository;
public class GenericRepository<T> where T : class
{
    private readonly PetShop2023DbContext _context;
    private readonly DbSet<T> dbSet;

    public GenericRepository(PetShop2023DbContext context)
    {
        _context = context;
        dbSet = context.Set<T>();
    }

    public async Task CreateAsync(T entity)
    {
        await dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<T?> FindOneAsync(
        Expression<Func<T, bool>> expression,
        Func<IQueryable<T>, IQueryable<T>>? includeFunc = null)
    {
        IQueryable<T> query = dbSet;

        if (includeFunc != null)
        {
            query = includeFunc(query);
        }

        return await query.AsNoTracking().FirstOrDefaultAsync(expression);
    }

    public async Task<List<T>> FindAllAsync(
        Expression<Func<T, bool>>? expression = null,
        Func<IQueryable<T>, IQueryable<T>>? includeFunc = null)
    {
        IQueryable<T> query = dbSet;

        if (expression != null)
        {
            query = query.Where(expression);
        }

        if (includeFunc != null)
        {
            query = includeFunc(query);
        }

        return await query.AsNoTracking().ToListAsync();
    }

}
