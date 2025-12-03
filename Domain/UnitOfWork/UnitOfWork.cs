using API.Infrastructure.Persistence.DbContexts;

namespace API.Domain.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    public ApplicationDbContext _context { get; set; }
    
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    public async Task BeginTransactionAsync()
    {
        if (_context.Database.CurrentTransaction != null)
            return;
        
        await _context.Database.BeginTransactionAsync();
    }
    
    public async Task<bool> SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task CommitTransactionAsync()
    {
        if (_context.Database.CurrentTransaction == null)
            return;

        await _context.Database.CommitTransactionAsync();
    }

    public async Task CommitChangesAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            await CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            await RollbackTransactionAsync();
            throw;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_context.Database.CurrentTransaction == null)
            return;

        await _context.Database.RollbackTransactionAsync();
        _context.ChangeTracker.Clear();
    }
}

