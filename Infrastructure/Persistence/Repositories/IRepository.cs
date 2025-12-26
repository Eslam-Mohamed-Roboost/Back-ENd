using System.Linq.Expressions;
using API.Domain.Entities;

namespace API.Infrastructure.Persistence.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    IQueryable<T> Get(Expression<Func<T?, bool>> predicate = null);
    IQueryable<T?> GetNotDeleted();
    Task<T?> FirstOrDefaultAsync(long id, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T?, bool>> predicate = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<T?, bool>> predicate = null, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default);
    long Add(T entity);
    void Update(T entity);
    void SaveIncluded(T entity, params string[] properties);
    void Delete(T entity);
    void AddRange(IEnumerable<T> entities);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    void SaveChanges();
    Task SaveChangesAsync(CancellationToken? cancellationToken = null);
}

