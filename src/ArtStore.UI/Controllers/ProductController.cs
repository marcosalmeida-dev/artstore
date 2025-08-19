using ArtStore.Application.Features.Products.Queries.GetAll;
using ArtStore.Shared.DTOs.Product;
using ArtStore.Shared.Interfaces.Query;
using Microsoft.AspNetCore.Mvc;

namespace ArtStore.UI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IQueryHandler<GetAllProductsQuery, IEnumerable<ProductDto?>> _getAllProductsQueryHandler;
    public ProductController(IQueryHandler<GetAllProductsQuery, IEnumerable<ProductDto?>> getAllProductsQueryHandler)
    {
        _getAllProductsQueryHandler = getAllProductsQueryHandler;
    }

    [HttpGet("get-all-products")]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _getAllProductsQueryHandler.Handle(new GetAllProductsQuery());
        return Ok(products);
    }
}
