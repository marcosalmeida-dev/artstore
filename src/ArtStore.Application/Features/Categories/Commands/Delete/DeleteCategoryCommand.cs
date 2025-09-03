using ArtStore.Application.Common.Interfaces;
using ArtStore.Shared.Interfaces.Command;

namespace ArtStore.Application.Features.Categories.Commands.Delete;

public class DeleteCategoryCommand : ICommand<Result<int>>
{
    public int Id { get; set; }
}

public class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public DeleteCategoryCommandHandler(
        IApplicationDbContext context
    )
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.Categories.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (item == null)
        {
            return await Result<int>.FailureAsync($"Category with id: [{request.Id}] not found.");
        }

        // Check if category has products
        var hasProducts = await _context.Products.AnyAsync(p => p.Category.Id == request.Id, cancellationToken);
        if (hasProducts)
        {
            return await Result<int>.FailureAsync("Cannot delete category that has products assigned.");
        }

        // Check if category has child categories
        var hasChildCategories = await _context.Categories.AnyAsync(c => c.ParentCategoryId == request.Id, cancellationToken);
        if (hasChildCategories)
        {
            return await Result<int>.FailureAsync("Cannot delete category that has child categories.");
        }

        _context.Categories.Remove(item);
        item.AddDomainEvent(new DeletedEvent<Category>(item));
        
        await _context.SaveChangesAsync(cancellationToken);

        return await Result<int>.SuccessAsync(item.Id);
    }
}