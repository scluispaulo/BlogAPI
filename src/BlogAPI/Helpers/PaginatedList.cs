using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Helpers;

public class PaginatedList<T> : List<T>
{
    public int CurrentPage { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }
    public int TotalPages { get; private set; }

    public PaginatedList(List<T> source, int currentPage, int pageSize, int totalCount)
    {
        AddRange(source);
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int currentPage, int pageSize)
    {
        var totalCount = await source.CountAsync();
        var items = await source.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedList<T>(items, currentPage, pageSize, totalCount);
    }
}