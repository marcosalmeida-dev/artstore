using ArtStore.Application.Common.Interfaces;
using ArtStore.Application.Features.Products.Caching;
using ArtStore.Domain.Extensions;
using ArtStore.Shared.DTOs.Product;
using ArtStore.Shared.Interfaces.Query;

namespace ArtStore.Application.Features.Products.Queries.GetAll;

public class GetAllProductsQuery : IQuery<IEnumerable<ProductDto>>
{
    public string Culture { get; set; } = "pt-BR";

    public string CacheKey => ProductCacheKey.GetAllCacheKey + $"_{Culture}";
    public IEnumerable<string>? Tags => ProductCacheKey.Tags;
}

public class GetProductQuery : IQuery<ProductDto>
{
    public required int Id { get; set; }
    public string Culture { get; set; } = "pt-BR";

    public string CacheKey => ProductCacheKey.GetProductByIdCacheKey(Id) + $"_{Culture}";
    public IEnumerable<string>? Tags => ProductCacheKey.Tags;
}

public class GetAllProductsQueryHandler :
    IQueryHandler<GetAllProductsQuery, IEnumerable<ProductDto?>>,
    IQueryHandler<GetProductQuery, ProductDto?>

{
    private readonly IApplicationDbContext _context;

    public GetAllProductsQueryHandler(
        IApplicationDbContext context
    )
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductDto?>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Tenant)
            .Include(p => p.Pictures)
            .ToListAsync(cancellationToken);

        return data.Select(x => new ProductDto
        {
            Id = x.Id,
            Name = x.GetLocalizedName(request.Culture) ?? x.Name ?? string.Empty,
            Description = x.GetLocalizedDescription(request.Culture) ?? x.Description,
            Brand = x.Brand,
            Unit = x.GetLocalizedUnit(request.Culture) ?? x.Unit,
            Price = x.Price,
            IsActive = x.IsActive,
            CategoryId = x.CategoryId,
            CategoryName = x.Category?.GetLocalizedName(request.Culture) ?? x.Category?.Name,
            TenantId = x.TenantId,
            TenantName = x.Tenant?.Name,
            ProductCode = x.ProductCode,
            Pictures = x.Pictures?.Select(p => new ProductImageDto
            {
                Id = p.Id,
                Name = p.Name,
                FileName = p.FileName,
                Url = p.Url,
                AltText = p.AltText,
                Caption = p.Caption,
                Size = p.Size,
                Width = p.Width,
                Height = p.Height,
                MimeType = p.MimeType,
                IsPrimary = p.IsPrimary,
                SortOrder = p.SortOrder
            }).OrderBy(p => p.SortOrder).ToList() ?? new List<ProductImageDto>(),
            Created = x.Created,
            LastModified = x.LastModified
        });
    }

    public async Task<ProductDto?> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Tenant)
            .Include(p => p.Pictures)
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return data == null ? null : new ProductDto
        {
            Id = data.Id,
            Name = data.GetLocalizedName(request.Culture) ?? data.Name ?? string.Empty,
            Description = data.GetLocalizedDescription(request.Culture) ?? data.Description,
            Brand = data.Brand,
            Unit = data.GetLocalizedUnit(request.Culture) ?? data.Unit,
            Price = data.Price,
            IsActive = data.IsActive,
            CategoryId = data.CategoryId,
            CategoryName = data.Category?.GetLocalizedName(request.Culture) ?? data.Category?.Name,
            TenantId = data.TenantId,
            TenantName = data.Tenant?.Name,
            ProductCode = data.ProductCode,
            Pictures = data.Pictures?.Select(p => new ProductImageDto
            {
                Id = p.Id,
                Name = p.Name,
                FileName = p.FileName,
                Url = p.Url,
                AltText = p.AltText,
                Caption = p.Caption,
                Size = p.Size,
                Width = p.Width,
                Height = p.Height,
                MimeType = p.MimeType,
                IsPrimary = p.IsPrimary,
                SortOrder = p.SortOrder
            }).OrderBy(p => p.SortOrder).ToList() ?? new List<ProductImageDto>(),
            Created = data.Created,
            LastModified = data.LastModified
        };
    }
}