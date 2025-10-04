using ArtStore.Application.Common.Interfaces;
using ArtStore.Application.Features.Products.Services;
using ArtStore.Application.Interfaces.Services;
using ArtStore.Shared.DTOs.Product.Commands;
using ArtStore.Shared.Interfaces.Command;

namespace ArtStore.Application.Features.Products.Commands.AddEdit;

public class AddEditProductCommandHandler : ICommandHandler<AddEditProductCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IProductCacheService _cacheService;
    private readonly IBlobStorageService _blobStorageService;

    public AddEditProductCommandHandler(
        IApplicationDbContext context,
        IProductCacheService cacheService,
        IBlobStorageService blobStorageService
    )
    {
        _context = context;
        _cacheService = cacheService;
        _blobStorageService = blobStorageService;
    }

    public async Task<Result<int>> Handle(AddEditProductCommand request, CancellationToken cancellationToken)
    {
        if (request.Id > 0)
        {
            var item = await _context.Products
                .Include(x => x.ProductImages)
                .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (item == null)
            {
                return await Result<int>.FailureAsync($"Product with id: [{request.Id}] not found.");
            }

            item.Name = request.Name;
            item.Description = request.Description;
            item.Brand = request.Brand;
            item.Unit = request.Unit;
            item.Price = request.Price;
            item.IsActive = request.IsActive;
            item.CategoryId = request.CategoryId;
            item.ProductCode = request.ProductCode;

            // Handle image deletions
            if (request.ImagesToDelete?.Any() == true)
            {
                var imagesToRemove = item.ProductImages.Where(p => request.ImagesToDelete.Contains(p.Id)).ToList();
                foreach (var imageToRemove in imagesToRemove)
                {
                    // Delete from blob storage (FileName now contains the full path with folder)
                    try
                    {
                        await _blobStorageService.DeleteFileAsync("product-images", imageToRemove.FileName, cancellationToken);
                    }
                    catch
                    {
                        // Continue even if blob deletion fails
                    }
                    item.ProductImages.Remove(imageToRemove);
                }
            }

            // Handle existing image updates (IsPrimary changes)
            if (request.ImagesToUpdate?.Any() == true)
            {
                foreach (var imageUpdate in request.ImagesToUpdate)
                {
                    var existingImage = item.ProductImages.FirstOrDefault(i => i.Id == imageUpdate.Id);
                    if (existingImage != null)
                    {
                        existingImage.IsPrimary = imageUpdate.IsPrimary;
                    }
                }
            }

            // Handle new image uploads
            if (request.NewImages?.Any() == true)
            {
                var sanitizedProductName = SanitizeFolderName(item.Name);
                foreach (var newImage in request.NewImages)
                {
                    var uniqueFileName = $"{Guid.NewGuid()}_{newImage.FileName}";
                    var blobPath = $"{sanitizedProductName}/{uniqueFileName}";
                    var uploadResult = await _blobStorageService.UploadFileAsync(
                        $"product-images",
                        blobPath,
                        newImage.Content,
                        newImage.ContentType,
                        newImage.Width,
                        newImage.Height,
                        cancellationToken);

                    item.ProductImages.Add(new ProductImage
                    {
                        Name = newImage.Name,
                        FileName = blobPath,
                        Url = uploadResult.Url,
                        Size = newImage.Content.Length,
                        AltText = newImage.AltText,
                        Caption = newImage.Caption,
                        MimeType = newImage.ContentType,
                        IsPrimary = newImage.IsPrimary,
                        SortOrder = newImage.SortOrder,
                        Width = newImage.Width > 0 ? newImage.Width : uploadResult.Width,
                        Height = newImage.Height > 0 ? newImage.Height : uploadResult.Height
                    });
                }
            }

            item.AddDomainEvent(new UpdatedEvent<Product>(item));

            await _context.SaveChangesAsync(cancellationToken);

            await _cacheService.InvalidateAllProductCacheAsync();

            return await Result<int>.SuccessAsync(item.Id);
        }
        else
        {
            var item = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Brand = request.Brand,
                Unit = request.Unit,
                Price = request.Price,
                IsActive = request.IsActive,
                CategoryId = request.CategoryId,
                ProductCode = request.ProductCode
            };

            // Handle new image uploads
            if (request.NewImages?.Any() == true)
            {
                var sanitizedProductName = SanitizeFolderName(item.Name);
                var productImages = new List<ProductImage>();
                foreach (var newImage in request.NewImages)
                {
                    var uniqueFileName = $"{Guid.NewGuid()}_{newImage.FileName}";
                    var blobPath = $"{sanitizedProductName}/{uniqueFileName}";
                    var uploadResult = await _blobStorageService.UploadFileAsync(
                        $"product-images",
                        blobPath,
                        newImage.Content,
                        newImage.ContentType,
                        newImage.Width,
                        newImage.Height,
                        cancellationToken);

                    productImages.Add(new ProductImage
                    {
                        Name = newImage.Name,
                        FileName = blobPath,
                        Url = uploadResult.Url,
                        Size = newImage.Content.Length,
                        AltText = newImage.AltText,
                        Caption = newImage.Caption,
                        MimeType = newImage.ContentType,
                        IsPrimary = newImage.IsPrimary,
                        SortOrder = newImage.SortOrder,
                        Width = newImage.Width > 0 ? newImage.Width : uploadResult.Width,
                        Height = newImage.Height > 0 ? newImage.Height : uploadResult.Height
                    });
                }
                item.ProductImages = productImages;
            }

            item.AddDomainEvent(new CreatedEvent<Product>(item));

            _context.Products.Add(item);
            await _context.SaveChangesAsync(cancellationToken);

            // Invalidate cache for all products since a new product was added
            await _cacheService.InvalidateAllProductCacheAsync();

            return await Result<int>.SuccessAsync(item.Id);
        }
    }

    private static string SanitizeFolderName(string name)
    {
        // Remove invalid characters for folder names
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = string.Join("_", name.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));

        // Replace spaces with hyphens and remove multiple consecutive underscores/hyphens
        sanitized = sanitized.Replace(" ", "-");
        sanitized = System.Text.RegularExpressions.Regex.Replace(sanitized, @"[-_]+", "-");

        // Trim and limit length
        sanitized = sanitized.Trim('-', '_');
        if (sanitized.Length > 50)
        {
            sanitized = sanitized.Substring(0, 50).TrimEnd('-', '_');
        }

        return string.IsNullOrWhiteSpace(sanitized) ? "product" : sanitized.ToLowerInvariant();
    }
}