using ArtStore.Application.Common.Interfaces;
using ArtStore.Application.Common.Models;
using ArtStore.Application.Features.Categories.Caching;
using ArtStore.Shared.DTOs.Category;
using ArtStore.Shared.Interfaces.Query;
using Microsoft.EntityFrameworkCore;

namespace ArtStore.Application.Features.Categories.Queries.Search;

public class SearchCategoriesQuery : IQuery<PaginatedData<CategoryDto>>
{
    public string? SearchString { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? OrderBy { get; set; } = "Name";
    public string? SortDirection { get; set; } = "asc";

    public string CacheKey => CategoryCacheKey.GetSearchCacheKey($"{SearchString}-{IsActive}-{PageNumber}-{PageSize}-{OrderBy}-{SortDirection}");
    public IEnumerable<string>? Tags => CategoryCacheKey.Tags;
}

public class SearchCategoriesQueryHandler : IQueryHandler<SearchCategoriesQuery, PaginatedData<CategoryDto>>
{
    private readonly IApplicationDbContext _context;

    public SearchCategoriesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedData<CategoryDto>> Handle(SearchCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Categories
            .Include(c => c.ParentCategory)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchString))
        {
            query = query.Where(c => c.Name.Contains(request.SearchString) ||
                                   c.Description!.Contains(request.SearchString));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(c => c.IsActive == request.IsActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        var sortBy = request.OrderBy?.ToLowerInvariant() ?? "name";
        var ascending = request.SortDirection?.ToLowerInvariant() != "desc";

        query = sortBy switch
        {
            "name" => ascending ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name),
            "description" => ascending ? query.OrderBy(c => c.Description) : query.OrderByDescending(c => c.Description),
            "isactive" => ascending ? query.OrderBy(c => c.IsActive) : query.OrderByDescending(c => c.IsActive),
            "parentcategoryname" => ascending ? query.OrderBy(c => c.ParentCategory!.Name) : query.OrderByDescending(c => c.ParentCategory!.Name),
            "productcount" => ascending ? query.OrderBy(c => c.Products!.Count) : query.OrderByDescending(c => c.Products!.Count),
            "created" => ascending ? query.OrderBy(c => c.Created) : query.OrderByDescending(c => c.Created),
            _ => query.OrderBy(c => c.Name)
        };

        var categories = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new CategoryDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                IsActive = x.IsActive,
                ParentCategoryId = x.ParentCategoryId,
                ParentCategoryName = x.ParentCategory!.Name,
                ProductCount = x.Products!.Count,
                Created = x.Created,
                LastModified = x.LastModified
            })
            .ToListAsync(cancellationToken);

        return new PaginatedData<CategoryDto>(categories, totalCount, request.PageNumber, request.PageSize);
    }
}