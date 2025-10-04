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
public class InventoryLocationsController : ControllerBase
{
    private readonly IQueryHandler<GetAllInventoryLocationsQuery, IEnumerable<InventoryLocationDto?>> _getAllLocationsQueryHandler;
    private readonly IQueryHandler<GetInventoryLocationQuery, InventoryLocationDto?> _getLocationQueryHandler;
    private readonly ICommandHandler<AddEditInventoryLocationCommand, Result<int>> _addEditLocationCommandHandler;
    private readonly ICommandHandler<DeleteInventoryLocationCommand, Result<int>> _deleteLocationCommandHandler;

    public InventoryLocationsController(
        IQueryHandler<GetAllInventoryLocationsQuery, IEnumerable<InventoryLocationDto?>> getAllLocationsQueryHandler,
        IQueryHandler<GetInventoryLocationQuery, InventoryLocationDto?> getLocationQueryHandler,
        ICommandHandler<AddEditInventoryLocationCommand, Result<int>> addEditLocationCommandHandler,
        ICommandHandler<DeleteInventoryLocationCommand, Result<int>> deleteLocationCommandHandler)
    {
        _getAllLocationsQueryHandler = getAllLocationsQueryHandler;
        _getLocationQueryHandler = getLocationQueryHandler;
        _addEditLocationCommandHandler = addEditLocationCommandHandler;
        _deleteLocationCommandHandler = deleteLocationCommandHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllLocations()
    {
        var locations = await _getAllLocationsQueryHandler.Handle(new GetAllInventoryLocationsQuery());
        return Ok(locations);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLocation(int id)
    {
        var location = await _getLocationQueryHandler.Handle(new GetInventoryLocationQuery { Id = id });
        if (location == null)
        {
            return NotFound();
        }

        return Ok(location);
    }

    [HttpPost]
    public async Task<IActionResult> CreateLocation([FromBody] AddEditInventoryLocationCommand command)
    {
        command.Id = 0; // Ensure it's a create operation
        var result = await _addEditLocationCommandHandler.Handle(command);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result.Errors);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLocation(int id, [FromBody] AddEditInventoryLocationCommand command)
    {
        command.Id = id;
        var result = await _addEditLocationCommandHandler.Handle(command);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result.Errors);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLocation(int id)
    {
        var command = new DeleteInventoryLocationCommand { Id = id };
        var result = await _deleteLocationCommandHandler.Handle(command);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result.Errors);
    }
}