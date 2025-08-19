using ArtStore.Application.Common.Interfaces;
using ArtStore.Shared.Interfaces.Command;
using Microsoft.AspNetCore.Components.Forms;

namespace ArtStore.Application.Features.Products.Commands.AddEdit;

public class AddEditProductCommand : ICommand<Result<int>>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Unit { get; set; }
    public string? Brand { get; set; }
    public decimal Price { get; set; }
    public List<ProductImage>? Pictures { get; set; }

    public IReadOnlyList<IBrowserFile>? UploadPictures { get; set; }
}

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
                return await Result<int>.FailureAsync($"Prduct with id: [{request.Id}] not found.");
            }
            item = new Product
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                Unit = request.Unit,
                Brand = request.Brand,
                Price = request.Price,
                Pictures = request.Pictures ?? new List<ProductImage>()
            };
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
                Unit = request.Unit,
                Brand = request.Brand,
                Price = request.Price,
                Pictures = request.Pictures ?? new List<ProductImage>()
            };
            item.AddDomainEvent(new CreatedEvent<Product>(item));

            _context.Products.Add(item);
            await _context.SaveChangesAsync(cancellationToken);

            return await Result<int>.SuccessAsync(item.Id);
        }
    }
}