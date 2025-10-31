using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace GarryLibrary.Data;

public interface IDataRepository<T> where T : class
{
    Task<T?> GetAsync(params object[] keyValues);
    Task<List<T>> GetAllAsync(Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);
    Task CreateAsync(T item);
    Task UpdateAsync(T item);
    Task DeleteAsync(T item);
    Task DeleteAllAsync(Expression<Func<T, bool>> predicate);
    Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
}