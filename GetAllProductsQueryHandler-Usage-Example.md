# GetAllProductsQueryHandler with Translation Support - Usage Examples

This document demonstrates how to use the updated `GetAllProductsQueryHandler` with multi-language support.

## Updated Features

The `GetAllProductsQueryHandler` now supports:
- **Culture Parameter**: Specify language code for localized content
- **Product Translation Mapping**: Automatically maps translated properties
- **Category Translation Mapping**: Includes localized category names
- **Fallback Support**: Falls back to default language when translation not available
- **Cache Key Culture Support**: Separate cache entries per culture

## Usage Examples

### Basic Usage with Different Cultures

```csharp
// Get products in English (default)
var queryEn = new GetAllProductsQuery { Culture = "en" };
var productsEn = await mediator.Send(queryEn);

// Get products in Portuguese (Brazil)
var queryPtBr = new GetAllProductsQuery { Culture = "pt-BR" };
var productsPtBr = await mediator.Send(queryPtBr);

// Get products in Spanish (Argentina)
var queryEsAr = new GetAllProductsQuery { Culture = "es-AR" };
var productsEsAr = await mediator.Send(queryEsAr);
```

### Controller Implementation

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(
        [FromQuery] string culture = "en")
    {
        var query = new GetAllProductsQuery { Culture = culture };
        var products = await _mediator.Send(query);
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> Get(int id, 
        [FromQuery] string culture = "en")
    {
        var query = new GetProductQuery { Id = id, Culture = culture };
        var product = await _mediator.Send(query);
        return product == null ? NotFound() : Ok(product);
    }
}
```

### Expected Results by Culture

#### English (en)
```json
{
  "id": 1,
  "name": "Regular Sugarcane Juice - Pure",
  "description": "Fresh regular sugarcane juice with pure flavor",
  "unit": "300ml Cup",
  "categoryName": "Drinks",
  "price": 8.00
}
```

#### Portuguese Brazil (pt-BR)
```json
{
  "id": 1,
  "name": "Caldo de Cana Normal - Puro",
  "description": "Caldo de cana fresco normal com sabor puro",
  "unit": "Copo 300ml",
  "categoryName": "Bebidas",
  "price": 8.00
}
```

#### Spanish Argentina (es-AR)
```json
{
  "id": 1,
  "name": "Jugo de Caña Normal - Puro",
  "description": "Jugo de caña fresco normal con sabor puro",
  "unit": "Vaso 300ml",
  "categoryName": "Bebidas",
  "price": 8.00
}
```

### Frontend Integration

```javascript
// JavaScript/TypeScript example
class ProductService {
    async getProducts(culture = 'en') {
        const response = await fetch(`/api/products?culture=${culture}`);
        return await response.json();
    }

    async getProduct(id, culture = 'en') {
        const response = await fetch(`/api/products/${id}?culture=${culture}`);
        return await response.json();
    }
}

// Usage in React/Angular/Vue component
const productService = new ProductService();

// Get products based on user's language preference
const userLanguage = navigator.language; // e.g., "pt-BR"
const products = await productService.getProducts(userLanguage);
```

### Blazor Component Usage

```csharp
@page "/products"
@inject IMediator Mediator

<select @bind="selectedCulture" @onchange="LoadProducts">
    <option value="en">English</option>
    <option value="pt-BR">Português (Brasil)</option>
    <option value="es-AR">Español (Argentina)</option>
</select>

<div class="products-grid">
    @foreach (var product in products)
    {
        <div class="product-card">
            <h3>@product.Name</h3>
            <p>@product.Description</p>
            <p><strong>@product.CategoryName</strong></p>
            <p>Unit: @product.Unit</p>
            <p>Price: @product.Price.ToString("C")</p>
        </div>
    }
</div>

@code {
    private string selectedCulture = "en";
    private List<ProductDto> products = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadProducts();
    }

    private async Task LoadProducts()
    {
        var query = new GetAllProductsQuery { Culture = selectedCulture };
        var result = await Mediator.Send(query);
        products = result?.ToList() ?? new();
        StateHasChanged();
    }
}
```

## Translation Mapping Logic

The handler uses the following mapping priority:

1. **Product Name**: `GetLocalizedName(culture)` → fallback to `Name`
2. **Product Description**: `GetLocalizedDescription(culture)` → fallback to `Description`
3. **Product Unit**: `GetLocalizedUnit(culture)` → fallback to `Unit`
4. **Category Name**: `Category.GetLocalizedName(culture)` → fallback to `Category.Name`

## Cache Behavior

- **Culture-Specific Caching**: Each culture has its own cache entry
- **Cache Keys**: `ProductsCacheKey_en`, `ProductsCacheKey_pt-BR`, `ProductsCacheKey_es-AR`
- **Performance**: Improves response time for repeated requests in the same language

## Benefits

1. **SEO Friendly**: Different URLs can serve different languages
2. **Performance**: Cached responses per culture
3. **Flexible**: Easy to add new languages
4. **Fallback Support**: Graceful degradation when translations missing
5. **Type Safe**: Strongly typed culture parameters