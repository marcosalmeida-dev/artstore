using ArtStore.Application.Common.Interfaces;
using ArtStore.Shared.DTOs.Category.Commands;
using ArtStore.Shared.Interfaces.Command;

namespace ArtStore.Application.Features.Categories.Commands.AddEdit;

public class AddEditCategoryCommandHandler : ICommandHandler<AddEditCategoryCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public AddEditCategoryCommandHandler(
        IApplicationDbContext context
    )
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(AddEditCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request.Id > 0)
        {
            var item = await _context.Categories.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (item == null)
            {
                return await Result<int>.FailureAsync($"Category with id: [{request.Id}] not found.");
            }
            
            item.Name = request.Name;
            item.Description = request.Description;
            item.IsActive = request.IsActive;
            item.ParentCategoryId = request.ParentCategoryId;

            item.AddDomainEvent(new UpdatedEvent<Category>(item));

            await _context.SaveChangesAsync(cancellationToken);

            return await Result<int>.SuccessAsync(item.Id);
        }
        else
        {
            var item = new Category
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive,
                ParentCategoryId = request.ParentCategoryId
            };
            item.AddDomainEvent(new CreatedEvent<Category>(item));

            _context.Categories.Add(item);
            await _context.SaveChangesAsync(cancellationToken);

            return await Result<int>.SuccessAsync(item.Id);
        }
    }
}