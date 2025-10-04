using System.Text.Json.Serialization;

namespace ArtStore.Application.Common.Models;

public class PaginatedData<T>
{
    [JsonConstructor]
    public PaginatedData(IEnumerable<T> items, int totalItems, int currentPage, int totalPages)
    {
        Items = items;
        TotalItems = totalItems;
        CurrentPage = currentPage;
        TotalPages = totalPages;
    }
    public IEnumerable<T> Items { get; set; } = Array.Empty<T>();
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
    public static PaginatedData<T> Create(IEnumerable<T> items, int total, int pageIndex, int pageSize)
        => new(items, total, pageIndex, (int)Math.Ceiling(total / (double)pageSize));
}