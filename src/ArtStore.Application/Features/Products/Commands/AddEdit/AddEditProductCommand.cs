using ArtStore.Application.Common.Interfaces;
using ArtStore.Shared.DTOs.Product.Commands;
using ArtStore.Shared.Interfaces.Command;

namespace ArtStore.Application.Features.Products.Commands.AddEdit;

public class AddEditProductCommandHandler : ICommandHandler<AddEditProductCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public AddEditProductCommandHandler(
        IApplicationDbContext context
    )
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(AddEditProductCommand request, CancellationToken cancellationToken)
    {
        if (request.Id > 0)
        {
            var item = await _context.Products.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
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

            if (request.Pictures?.Any() == true)
            {
                item.Pictures = request.Pictures.Select(p => new ProductImage
                {
                    Name = p.Name,
                    Size = p.Size,
                    Url = p.Url
                }).ToList();
            }

            item.AddDomainEvent(new UpdatedEvent<Product>(item));

            await _context.SaveChangesAsync(cancellationToken);

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
                CategoryId = request.CategoryId
            };

            if (request.Pictures?.Any() == true)
            {
                item.Pictures = request.Pictures.Select(p => new ProductImage
                {
                    Name = p.Name,
                    Size = p.Size,
                    Url = p.Url
                }).ToList();
            }

            item.AddDomainEvent(new CreatedEvent<Product>(item));

            _context.Products.Add(item);
            await _context.SaveChangesAsync(cancellationToken);

            return await Result<int>.SuccessAsync(item.Id);
        }
    }
}