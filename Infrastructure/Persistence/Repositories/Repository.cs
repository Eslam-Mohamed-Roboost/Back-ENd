using System.Linq.Expressions;
using API.Domain.Entities;
using API.Infrastructure.Persistence.DbContexts;
using API.Shared.Models;
using IdGen;
using Microsoft.EntityFrameworkCore;

namespace API.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity, new()
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;
    private readonly IdGenerator _idGenerator;
    private readonly UserState _userState;

    public Repository(ApplicationDbContext context, IdGenerator idGenerator, UserState userState)
    {
        _context = context;
        _dbSet = _context.Set<T>();
        _idGenerator = idGenerator;
        _userState = userState;
    }

    public IQueryable<T> Get(Expression<Func<T?, bool>> predicate = null)
    {
        return predicate == null 
            ? _dbSet.Where(entity => !entity.IsDeleted) 
            : _dbSet.Where(entity => !entity.IsDeleted).Where(predicate);
    }

    public IQueryable<T?> GetNotDeleted()
    {
        return _dbSet.Where(e => !e.IsDeleted);
    }

    public async Task<T?> FirstOrDefaultAsync(long id, CancellationToken cancellationToken = default)
    {
        return await GetNotDeleted().FirstOrDefaultAsync(e => e.ID == id, cancellationToken).ConfigureAwait(false);
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T?, bool>> predicate = null, CancellationToken cancellationToken = default)
    {
        return predicate == null 
            ? await GetNotDeleted().FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false) 
            : await GetNotDeleted().Where(predicate).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default)
    {
        return await GetNotDeleted().AnyAsync(e => e.ID == id, cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> ExistsAsync(Expression<Func<T?, bool>> predicate = null, CancellationToken cancellationToken = default)
    {
        return predicate == null 
            ? await GetNotDeleted().AnyAsync(cancellationToken) 
            : await GetNotDeleted().AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default)
    {
        return predicate == null 
            ? await GetNotDeleted().CountAsync(cancellationToken) 
            : await GetNotDeleted().CountAsync(predicate, cancellationToken);
    }

    public long Add(T entity)
    {
        entity.ID = _idGenerator.CreateId();
        entity.CreatedBy = _userState.UserID;
        entity.CompanyID = entity.CompanyID == 0 ? _userState.CompanyID : entity.CompanyID;
        _dbSet.Add(entity);
        return entity.ID;
    }

    public void Update(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = _userState.UserID;
        _dbSet.Attach(entity);

        var entry = _context.Entry(entity);
        entry.Property(e => e.CreatedAt).IsModified = false;
        entry.Property(e => e.CreatedBy).IsModified = false;
        entry.State = EntityState.Modified;
    }

    public void SaveIncluded(T entity, params string[] properties)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = _userState.UserID;
        
        _dbSet.Attach(entity);
        var entry = _context.Entry(entity);
        
        foreach (var prop in properties)
        {
            entry.Property(prop).IsModified = true;
        }
        
        entry.Property(e => e.UpdatedAt).IsModified = true;
        entry.Property(e => e.UpdatedBy).IsModified = true;
    }

    public void Delete(T entity)
    {
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = _userState.UserID;
        Update(entity);
    }

    public void AddRange(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            Add(entity);
        }
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await GetNotDeleted().AnyAsync(predicate);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }



    public async Task SaveChangesAsync(CancellationToken? cancellationToken = null)
    {
        await _context.SaveChangesAsync();
    }
}

