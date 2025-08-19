using ArtStore.Application.Common.Interfaces;
using ArtStore.Shared.Interfaces.Command;
using ArtStore.Shared.Interfaces.MultiTenant;

namespace ArtStore.Application.Features.Tenants.Commands.AddEdit;

public class AddEditTenantCommand : ICommand<Result<int>>
{
    [Description("Tenant Id")] public string Id { get; set; } = Guid.NewGuid().ToString();
    [Description("Tenant Name")] public string? Name { get; set; }
    [Description("Description")] public string? Description { get; set; }
}

public class AddEditTenantCommandHandler : ICommandHandler<AddEditTenantCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantService _tenantsService;

    public AddEditTenantCommandHandler(
        ITenantService tenantsService,
        IApplicationDbContext context
    )
    {
        _tenantsService = tenantsService;
        _context = context;
    }

    public async Task<Result<int>> Handle(AddEditTenantCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.Tenants.FindAsync(new object[] { request.Id }, cancellationToken);
        if (item is null)
        {
            item = new Tenant
            {
                Name = request.Name,
                Description = request.Description
            };
            await _context.Tenants.AddAsync(item, cancellationToken);
        }
        else
        {
            item.Name = request.Name;
            item.Description = request.Description;
        }

        await _context.SaveChangesAsync(cancellationToken);
        _tenantsService.Refresh();

        return await Result<int>.SuccessAsync(item.Id);
    }
}