using ArtStore.Application.Features.Products.Queries.GetAll;
using ArtStore.Shared.DTOs.Product;
using ArtStore.Shared.Interfaces.Query;

namespace ArtStore.UI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IQueryHandler<GetAllProductsQuery, IEnumerable<ProductDto?>> _getAllProductsQueryHandler;

    public ProductController(
        IQueryHandler<GetAllProductsQuery, IEnumerable<ProductDto?>> getAllProductsQueryHandler)
    {
        _getAllProductsQueryHandler = getAllProductsQueryHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts([FromQuery] string? culture = "pt-BR")
    {
        var query = new GetAllProductsQuery { Culture = culture ?? "pt-BR" };
        var products = await _getAllProductsQueryHandler.Handle(query);
        return Ok(products);
    }
}