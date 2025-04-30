using Microsoft.EntityFrameworkCore;

namespace Application.Features.Pagination;

public static class PaginationExtensions
{
    public static async Task<PaginationResult<T>> PaginateAsync<T>(this IQueryable<T> query, int pageNumber,
        int pageSize)
    {
        var totalCount = await query.CountAsync();

        if (totalCount == 0) return new PaginationResult<T>(new List<T>().AsQueryable(), 1, pageSize, 0, 0);

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        if (pageNumber < 1)
            pageNumber = 1;
        else if (pageNumber > totalPages)
            pageNumber = totalPages;

        var items = await query.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginationResult<T>(items.AsQueryable(), pageNumber, pageSize, totalCount, totalPages);
    }
}

public class PaginationResult<T>
{
    public PaginationResult(
        IQueryable<T> items,
        int pageNumber,
        int pageSize,
        int totalCount,
        int totalPages)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = totalPages;
    }

    public IQueryable<T> Items { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages { get; }
}