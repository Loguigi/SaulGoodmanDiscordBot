using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace GarryLibrary.Data;

public class GarryRepository<T> : IDataRepository<T> where T : class
{
    private readonly GarryDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GarryRepository(GarryDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetAsync(params object[] id) => await _dbSet.FindAsync(id);
    
    public async Task<List<T>> GetAllAsync(Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
    {
        IQueryable<T> query = _dbSet;
        
        if (include != null)
        {
            query = include(query);
        }
        
        return await query.ToListAsync();
    }

    public async Task CreateAsync(T item)
    {
        await _dbSet.AddAsync(item);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T item)
    {
        _dbSet.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T item)
    {
        _dbSet.Remove(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAllAsync(Expression<Func<T, bool>> predicate)
    {
        _dbSet.Where(predicate).ToList().ForEach(x => _dbSet.Remove(x));
        await _context.SaveChangesAsync();
    }

    public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
        await _dbSet.Where(predicate).ToListAsync();
}