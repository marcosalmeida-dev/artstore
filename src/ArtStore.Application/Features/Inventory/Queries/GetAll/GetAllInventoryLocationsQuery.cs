using ArtStore.Application.Common.Interfaces;
using ArtStore.Shared.DTOs.Inventory;
using ArtStore.Shared.Interfaces.Query;
using Microsoft.EntityFrameworkCore;

namespace ArtStore.Application.Features.Inventory.Queries.GetAll;

public class GetAllInventoryLocationsQuery : IQuery<IEnumerable<InventoryLocationDto>>
{
}

public class GetInventoryLocationQuery : IQuery<InventoryLocationDto>
{
    public required int Id { get; set; }
}

public class GetAllInventoryLocationsQueryHandler :
    IQueryHandler<GetAllInventoryLocationsQuery, IEnumerable<InventoryLocationDto?>>,
    IQueryHandler<GetInventoryLocationQuery, InventoryLocationDto?>
{
    private readonly IApplicationDbContext _context;

    public GetAllInventoryLocationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<InventoryLocationDto?>> Handle(GetAllInventoryLocationsQuery request, CancellationToken cancellationToken)
    {
        var locations = await _context.InventoryLocations
            .OrderBy(l => l.Name)
            .ToListAsync(cancellationToken);

        return locations.Select(l => new InventoryLocationDto
        {
            Id = l.Id,
            Name = l.Name,
            Code = l.Code,
            IsDefault = l.IsDefault,
            IsActive = l.IsActive,
            Created = l.Created,
            LastModified = l.LastModified
        }).ToList();
    }

    public async Task<InventoryLocationDto?> Handle(GetInventoryLocationQuery request, CancellationToken cancellationToken)
    {
        var location = await _context.InventoryLocations
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);
        if (location == null) return null;

        return new InventoryLocationDto
        {
            Id = location.Id,
            Name = location.Name,
            Code = location.Code,
            IsDefault = location.IsDefault,
            IsActive = location.IsActive,
            Created = location.Created,
            LastModified = location.LastModified
        };
    }
}