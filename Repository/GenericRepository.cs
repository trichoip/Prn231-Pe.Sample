using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Helpers;
using System.Linq.Expressions;

namespace Repository;
public class GenericRepository<T> where T : class
{
    private readonly PetShop2023DbContext _context;
    private readonly DbSet<T> dbSet;
    private readonly IMapper _mapper;

    public GenericRepository(PetShop2023DbContext context, IMapper mapper)
    {
        _context = context;
        dbSet = context.Set<T>();
        _mapper = mapper;
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

    public async Task<T?> FindByAsync(
        Expression<Func<T, bool>> expression,
        Func<IQueryable<T>, IQueryable<T>>? includeFunc = null)
    {
        IQueryable<T> query = dbSet;

        if (includeFunc != null)
        {
            query = includeFunc(query);
        }

        return await query.FirstOrDefaultAsync(expression);
    }

    public async Task<List<T>> FindAsync(
        Expression<Func<T, bool>>? expression = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
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
        if (orderBy != null)
        {
            query = orderBy(query);
        }
        return await query.ToListAsync();
    }

    public async Task<bool> ExistsByAsync(
        Expression<Func<T, bool>>? expression = null)
    {
        IQueryable<T> query = dbSet;

        if (expression != null)
        {
            query = query.Where(expression);
        }

        return await query.AnyAsync();
    }

    public async Task<PaginatedList<TDTO>> FindAsync<TDTO>(
        int pageIndex = 0,
        int pageSize = 0,
        Expression<Func<T, bool>>? expression = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null) where TDTO : class
    {
        IQueryable<T> query = dbSet;

        if (expression != null)
        {
            query = query.Where(expression);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return await query.ProjectTo<TDTO>(_mapper.ConfigurationProvider).PaginatedListAsync(pageIndex, pageSize);
    }

}
