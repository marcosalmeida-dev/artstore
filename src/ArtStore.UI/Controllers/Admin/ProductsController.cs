using ArtStore.Application.Common.Models;
using ArtStore.Application.Features.Products.Commands.Delete;
using ArtStore.Application.Features.Products.Queries.GetAll;
using ArtStore.Application.Features.Products.Queries.GetAllForManagement;
using ArtStore.Application.Features.Products.Queries.Search;
using ArtStore.Shared.DTOs.Product;
using ArtStore.Shared.DTOs.Product.Commands;
using ArtStore.Shared.Interfaces.Query;
using Microsoft.AspNetCore.Mvc;

namespace ArtStore.UI.Controllers.Admin;

[Route("api/admin/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IQueryHandler<GetAllProductsForManagementQuery, IEnumerable<ProductDto?>> _getAllProductsForManagementQueryHandler;
    private readonly IQueryHandler<GetProductQuery, ProductDto?> _getProductQueryHandler;
    private readonly IQueryHandler<SearchProductsQuery, PaginatedData<ProductDto>> _searchProductsQueryHandler;
    private readonly ICommandHandler<AddEditProductCommand, Result<int>> _addEditProductCommandHandler;
    private readonly ICommandHandler<DeleteProductCommand, Result<int>> _deleteProductCommandHandler;

    public ProductsController(
        IQueryHandler<GetAllProductsForManagementQuery, IEnumerable<ProductDto?>> getAllProductsForManagementQueryHandler,
        IQueryHandler<GetProductQuery, ProductDto?> getProductQueryHandler,
        IQueryHandler<SearchProductsQuery, PaginatedData<ProductDto>> searchProductsQueryHandler,
        ICommandHandler<AddEditProductCommand, Result<int>> addEditProductCommandHandler,
        ICommandHandler<DeleteProductCommand, Result<int>> deleteProductCommandHandler)
    {
        _getAllProductsForManagementQueryHandler = getAllProductsForManagementQueryHandler;
        _getProductQueryHandler = getProductQueryHandler;
        _searchProductsQueryHandler = searchProductsQueryHandler;
        _addEditProductCommandHandler = addEditProductCommandHandler;
        _deleteProductCommandHandler = deleteProductCommandHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts([FromQuery] string? culture = "pt-BR")
    {
        var query = new GetAllProductsForManagementQuery { Culture = culture ?? "pt-BR" };
        var products = await _getAllProductsForManagementQueryHandler.Handle(query);
        return Ok(products);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts([FromQuery] string? searchString, [FromQuery] bool? isActive, [FromQuery] int? categoryId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? orderBy = "Name", [FromQuery] string? sortDirection = "asc")
    {
        var query = new SearchProductsQuery
        {
            SearchString = searchString,
            IsActive = isActive,
            CategoryId = categoryId,
            PageNumber = pageNumber,
            PageSize = pageSize,
            OrderBy = orderBy,
            SortDirection = sortDirection
        };

        var result = await _searchProductsQueryHandler.Handle(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _getProductQueryHandler.Handle(new GetProductQuery { Id = id });
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] AddEditProductCommand command)
    {
        command.Id = 0; // Ensure it's a create operation
        var result = await _addEditProductCommandHandler.Handle(command);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result.Errors);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] AddEditProductCommand command)
    {
        command.Id = id;
        var result = await _addEditProductCommandHandler.Handle(command);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result.Errors);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var command = new DeleteProductCommand { Id = id };
        var result = await _deleteProductCommandHandler.Handle(command);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result.Errors);
    }
}