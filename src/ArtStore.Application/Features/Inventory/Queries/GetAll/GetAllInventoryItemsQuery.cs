using ArtStore.Application.Common.Interfaces;
using ArtStore.Application.Features.Inventory.Services;
using ArtStore.Domain.Entities.Enums;
using ArtStore.Shared.DTOs.Inventory;
using ArtStore.Shared.Interfaces.Query;
using Microsoft.EntityFrameworkCore;

namespace ArtStore.Application.Features.Inventory.Queries.GetAll;

public class GetAllInventoryItemsQuery : IQuery<IEnumerable<InventoryItemDto>>
{
}

public class GetInventoryItemQuery : IQuery<InventoryItemDto>
{
    public required long Id { get; set; }
}

public class GetAllInventoryItemsQueryHandler :
    IQueryHandler<GetAllInventoryItemsQuery, IEnumerable<InventoryItemDto?>>,
    IQueryHandler<GetInventoryItemQuery, InventoryItemDto?>
{
    private readonly IApplicationDbContext _context;

    public GetAllInventoryItemsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<InventoryItemDto?>> Handle(GetAllInventoryItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await _context.InventoryItems
            .Include(i => i.Product)
            .Include(i => i.Location)
            .OrderBy(i => i.Product.Name)
            .ThenBy(i => i.Location.Name)
            .ToListAsync(cancellationToken);

        var result = new List<InventoryItemDto>();

        foreach (var item in items)
        {
            // Get available quantity (OnHand - active reservations)
            var reserved = await _context.InventoryReservations
                .Where(r => r.ProductId == item.ProductId &&
                           r.InventoryLocationId == item.InventoryLocationId &&
                           r.Status == ReservationStatus.Active)
                .SumAsync(r => r.Quantity, cancellationToken);

            result.Add(new InventoryItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product.Name ?? string.Empty,
                ProductCode = item.Product.ProductCode,
                InventoryLocationId = item.InventoryLocationId,
                LocationName = item.Location.Name,
                OnHand = item.OnHand,
                SafetyStock = item.SafetyStock,
                ReorderPoint = item.ReorderPoint,
                Available = item.OnHand - reserved,
                Created = item.Created,
                LastModified = item.LastModified
            });
        }

        return result;
    }

    public async Task<InventoryItemDto?> Handle(GetInventoryItemQuery request, CancellationToken cancellationToken)
    {
        var item = await _context.InventoryItems
            .Include(i => i.Product)
            .Include(i => i.Location)
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
        if (item == null)
        {
            return null;
        }

        // Get available quantity (OnHand - active reservations)
        var reserved = await _context.InventoryReservations
            .Where(r => r.ProductId == item.ProductId &&
                       r.InventoryLocationId == item.InventoryLocationId &&
                       r.Status == ReservationStatus.Active)
            .SumAsync(r => r.Quantity, cancellationToken);

        return new InventoryItemDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.Product.Name ?? string.Empty,
            ProductCode = item.Product.ProductCode,
            InventoryLocationId = item.InventoryLocationId,
            LocationName = item.Location.Name,
            OnHand = item.OnHand,
            SafetyStock = item.SafetyStock,
            ReorderPoint = item.ReorderPoint,
            Available = item.OnHand - reserved,
            Created = item.Created,
            LastModified = item.LastModified
        };
    }
}