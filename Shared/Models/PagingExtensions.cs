using Microsoft.EntityFrameworkCore;

namespace API.Shared.Models;

public static class PagingExtensions
{
    public static async Task<PagingDto<T>> ToPagingDtoAsync<T>(
        this IQueryable<T> query, 
        int pageIndex, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var totalRecords = await query.CountAsync(cancellationToken);
        
        if (totalRecords == 0)
            return PagingDto<T>.Empty(pageSize);

        var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        
        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagingDto<T>(pageSize, pageIndex, totalRecords, totalPages, items);
    }
}

