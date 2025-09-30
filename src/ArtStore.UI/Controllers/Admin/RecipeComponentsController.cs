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
public class RecipeComponentsController : ControllerBase
{
    private readonly IQueryHandler<GetAllRecipeComponentsQuery, IEnumerable<RecipeComponentDto?>> _getAllComponentsQueryHandler;
    private readonly IQueryHandler<GetRecipeComponentQuery, RecipeComponentDto?> _getComponentQueryHandler;
    private readonly IQueryHandler<GetRecipeComponentsByProductQuery, IEnumerable<RecipeComponentDto?>> _getComponentsByProductQueryHandler;
    private readonly ICommandHandler<AddEditRecipeComponentCommand, Result<long>> _addEditComponentCommandHandler;
    private readonly ICommandHandler<DeleteRecipeComponentCommand, Result<long>> _deleteComponentCommandHandler;

    public RecipeComponentsController(
        IQueryHandler<GetAllRecipeComponentsQuery, IEnumerable<RecipeComponentDto?>> getAllComponentsQueryHandler,
        IQueryHandler<GetRecipeComponentQuery, RecipeComponentDto?> getComponentQueryHandler,
        IQueryHandler<GetRecipeComponentsByProductQuery, IEnumerable<RecipeComponentDto?>> getComponentsByProductQueryHandler,
        ICommandHandler<AddEditRecipeComponentCommand, Result<long>> addEditComponentCommandHandler,
        ICommandHandler<DeleteRecipeComponentCommand, Result<long>> deleteComponentCommandHandler)
    {
        _getAllComponentsQueryHandler = getAllComponentsQueryHandler;
        _getComponentQueryHandler = getComponentQueryHandler;
        _getComponentsByProductQueryHandler = getComponentsByProductQueryHandler;
        _addEditComponentCommandHandler = addEditComponentCommandHandler;
        _deleteComponentCommandHandler = deleteComponentCommandHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllComponents()
    {
        var components = await _getAllComponentsQueryHandler.Handle(new GetAllRecipeComponentsQuery());
        return Ok(components);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetComponent(long id)
    {
        var component = await _getComponentQueryHandler.Handle(new GetRecipeComponentQuery { Id = id });
        if (component == null)
        {
            return NotFound();
        }

        return Ok(component);
    }

    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetComponentsByProduct(int productId)
    {
        var components = await _getComponentsByProductQueryHandler.Handle(new GetRecipeComponentsByProductQuery { ProductId = productId });
        return Ok(components);
    }

    [HttpPost]
    public async Task<IActionResult> CreateComponent([FromBody] AddEditRecipeComponentCommand command)
    {
        command.Id = 0; // Ensure it's a create operation
        var result = await _addEditComponentCommandHandler.Handle(command);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result.Errors);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComponent(long id, [FromBody] AddEditRecipeComponentCommand command)
    {
        command.Id = id;
        var result = await _addEditComponentCommandHandler.Handle(command);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result.Errors);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComponent(long id)
    {
        var command = new DeleteRecipeComponentCommand { Id = id };
        var result = await _deleteComponentCommandHandler.Handle(command);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result.Errors);
    }
}