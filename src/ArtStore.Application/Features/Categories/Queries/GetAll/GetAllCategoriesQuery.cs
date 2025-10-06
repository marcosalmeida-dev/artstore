using ArtStore.Application.Common.Interfaces;
using ArtStore.Application.Features.Categories.Caching;
using ArtStore.Shared.DTOs.Category;
using ArtStore.Shared.Interfaces.Query;
using Microsoft.EntityFrameworkCore;

namespace ArtStore.Application.Features.Categories.Queries.GetAll;

public class GetAllCategoriesQuery : IQuery<IEnumerable<CategoryDto>>
{
    public string CacheKey => CategoryCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => CategoryCacheKey.Tags;
}

public class GetCategoryQuery : IQuery<CategoryDto>
{
    public required int Id { get; set; }

    public string CacheKey => CategoryCacheKey.GetCategoryByIdCacheKey(Id);
    public IEnumerable<string>? Tags => CategoryCacheKey.Tags;
}

public class GetAllCategoriesQueryHandler :
    IQueryHandler<GetAllCategoriesQuery, IEnumerable<CategoryDto?>>,
    IQueryHandler<GetCategoryQuery, CategoryDto?>
{
    private readonly IApplicationDbContext _context;

    public GetAllCategoriesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CategoryDto?>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(p => p.Products)
            .Where(c => c.IsActive) // Only active categories
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

        return data
            .Select(x => new CategoryDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                IsActive = x.IsActive,
                ParentCategoryId = x.ParentCategoryId,
                ParentCategoryName = x.ParentCategory?.Name,
                ProductCount = x.Products?.Count ?? 0,
                Created = x.Created,
                LastModified = x.LastModified
            })
            .OrderBy(c => c.Name); // Defensive ordering post-projection
    }

    public async Task<CategoryDto?> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(p => p.Products)
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return data == null ? null : new CategoryDto
        {
            Id = data.Id,
            Name = data.Name,
            Description = data.Description,
            IsActive = data.IsActive,
            ParentCategoryId = data.ParentCategoryId,
            ParentCategoryName = data.ParentCategory?.Name,
            ProductCount = data.Products?.Count ?? 0,
            Created = data.Created,
            LastModified = data.LastModified
        };
    }
}