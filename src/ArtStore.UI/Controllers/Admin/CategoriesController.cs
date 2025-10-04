using ArtStore.Application.Common.Models;
using ArtStore.Application.Features.Categories.Commands.Delete;
using ArtStore.Application.Features.Categories.Queries.GetAll;
using ArtStore.Application.Features.Categories.Queries.Search;
using ArtStore.Shared.DTOs.Category;
using ArtStore.Shared.DTOs.Category.Commands;
using ArtStore.Shared.Interfaces.Query;
using Microsoft.AspNetCore.Mvc;

namespace ArtStore.UI.Controllers.Admin;

[Route("api/admin/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly IQueryHandler<GetAllCategoriesQuery, IEnumerable<CategoryDto?>> _getAllCategoriesQueryHandler;
    private readonly IQueryHandler<GetCategoryQuery, CategoryDto?> _getCategoryQueryHandler;
    private readonly IQueryHandler<SearchCategoriesQuery, PaginatedData<CategoryDto>> _searchCategoriesQueryHandler;
    private readonly ICommandHandler<AddEditCategoryCommand, Result<int>> _addEditCategoryCommandHandler;
    private readonly ICommandHandler<DeleteCategoryCommand, Result<int>> _deleteCategoryCommandHandler;

    public CategoriesController(
        IQueryHandler<GetAllCategoriesQuery, IEnumerable<CategoryDto?>> getAllCategoriesQueryHandler,
        IQueryHandler<GetCategoryQuery, CategoryDto?> getCategoryQueryHandler,
        IQueryHandler<SearchCategoriesQuery, PaginatedData<CategoryDto>> searchCategoriesQueryHandler,
        ICommandHandler<AddEditCategoryCommand, Result<int>> addEditCategoryCommandHandler,
        ICommandHandler<DeleteCategoryCommand, Result<int>> deleteCategoryCommandHandler)
    {
        _getAllCategoriesQueryHandler = getAllCategoriesQueryHandler;
        _getCategoryQueryHandler = getCategoryQueryHandler;
        _searchCategoriesQueryHandler = searchCategoriesQueryHandler;
        _addEditCategoryCommandHandler = addEditCategoryCommandHandler;
        _deleteCategoryCommandHandler = deleteCategoryCommandHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _getAllCategoriesQueryHandler.Handle(new GetAllCategoriesQuery());
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategory(int id)
    {
        var category = await _getCategoryQueryHandler.Handle(new GetCategoryQuery { Id = id });
        if (category == null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchCategories([FromQuery] string? searchString, [FromQuery] bool? isActive, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? orderBy = "Name", [FromQuery] string? sortDirection = "asc")
    {
        var query = new SearchCategoriesQuery
        {
            SearchString = searchString,
            IsActive = isActive,
            PageNumber = pageNumber,
            PageSize = pageSize,
            OrderBy = orderBy,
            SortDirection = sortDirection
        };

        var result = await _searchCategoriesQueryHandler.Handle(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] AddEditCategoryCommand command)
    {
        command.Id = 0; // Ensure it's a create operation
        var result = await _addEditCategoryCommandHandler.Handle(command);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result.Errors);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] AddEditCategoryCommand command)
    {
        command.Id = id;
        var result = await _addEditCategoryCommandHandler.Handle(command);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result.Errors);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var command = new DeleteCategoryCommand { Id = id };
        var result = await _deleteCategoryCommandHandler.Handle(command);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result.Errors);
    }
}