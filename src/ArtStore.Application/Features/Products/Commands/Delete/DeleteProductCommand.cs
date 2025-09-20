using ArtStore.Application.Common.Interfaces;
using ArtStore.Application.Features.Products.Services;
using ArtStore.Shared.Interfaces.Command;

namespace ArtStore.Application.Features.Products.Commands.Delete;

public class DeleteProductCommand : ICommand<Result<int>>
{
    public int Id { get; set; }
}

public class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IProductCacheService _cacheService;

    public DeleteProductCommandHandler(
        IApplicationDbContext context,
        IProductCacheService cacheService
    )
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<Result<int>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.Products.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (item == null)
        {
            return await Result<int>.FailureAsync($"Product with id: [{request.Id}] not found.");
        }

        // Check if product is referenced in orders
        var hasOrders = await _context.OrderDetails.AnyAsync(od => od.ProductId == request.Id, cancellationToken);
        if (hasOrders)
        {
            return await Result<int>.FailureAsync("Cannot delete product that is referenced in orders.");
        }

        _context.Products.Remove(item);
        item.AddDomainEvent(new DeletedEvent<Product>(item));

        await _context.SaveChangesAsync(cancellationToken);

        // Invalidate cache for all products since a product was deleted
        await _cacheService.InvalidateAllProductCacheAsync();

        return await Result<int>.SuccessAsync(item.Id);
    }
}