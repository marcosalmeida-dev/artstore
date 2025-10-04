using ArtStore.Application.Common.Models;
using ArtStore.Application.Features.Inventory.Commands.AddEdit;
using ArtStore.Application.Features.Inventory.Commands.Delete;
using ArtStore.Application.Features.Inventory.Queries.GetAll;
using ArtStore.Shared.DTOs.Inventory;
using ArtStore.Shared.DTOs.Inventory.Commands;
using ArtStore.Shared.Interfaces.Command;
using ArtStore.Shared.Interfaces.Query;
using Microsoft.AspNetCore.Mvc;

namespace ArtStore.UI.Controllers.Admin;

[Route("api/admin/[controller]")]
[ApiController]
public class InventoryItemsController : ControllerBase
{
    private readonly IQueryHandler<GetAllInventoryItemsQuery, IEnumerable<InventoryItemDto?>> _getAllItemsQueryHandler;
    private readonly IQueryHandler<GetInventoryItemQuery, InventoryItemDto?> _getItemQueryHandler;
    private readonly ICommandHandler<AddEditInventoryItemCommand, Result<long>> _addEditItemCommandHandler;
    private readonly ICommandHandler<DeleteInventoryItemCommand, Result<long>> _deleteItemCommandHandler;

    public InventoryItemsController(
        IQueryHandler<GetAllInventoryItemsQuery, IEnumerable<InventoryItemDto?>> getAllItemsQueryHandler,
        IQueryHandler<GetInventoryItemQuery, InventoryItemDto?> getItemQueryHandler,
        ICommandHandler<AddEditInventoryItemCommand, Result<long>> addEditItemCommandHandler,
        ICommandHandler<DeleteInventoryItemCommand, Result<long>> deleteItemCommandHandler)
    {
        _getAllItemsQueryHandler = getAllItemsQueryHandler;
        _getItemQueryHandler = getItemQueryHandler;
        _addEditItemCommandHandler = addEditItemCommandHandler;
        _deleteItemCommandHandler = deleteItemCommandHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllItems()
    {
        var items = await _getAllItemsQueryHandler.Handle(new GetAllInventoryItemsQuery());
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetItem(long id)
    {
        var item = await _getItemQueryHandler.Handle(new GetInventoryItemQuery { Id = id });
        if (item == null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> CreateItem([FromBody] AddEditInventoryItemCommand command)
    {
        command.Id = 0; // Ensure it's a create operation
        var result = await _addEditItemCommandHandler.Handle(command);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result.Errors);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateItem(long id, [FromBody] AddEditInventoryItemCommand command)
    {
        command.Id = id;
        var result = await _addEditItemCommandHandler.Handle(command);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result.Errors);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(long id)
    {
        var command = new DeleteInventoryItemCommand { Id = id };
        var result = await _deleteItemCommandHandler.Handle(command);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result.Errors);
    }
}