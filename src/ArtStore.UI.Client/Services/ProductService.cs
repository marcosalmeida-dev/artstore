using System.Net.Http.Json;
using ArtStore.Shared.DTOs.Product;

namespace ArtStore.UI.Client.Services;

public class ProductService
{
    private readonly HttpClient _httpClient;
    public ProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<List<ProductDto>> GetProductsAsync(string? culture = "pt-BR")
    {
        var response = await _httpClient.GetAsync($"api/product?culture={culture ?? "pt-BR"}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<ProductDto>>() ?? new List<ProductDto>();
    }
    public async Task<ProductDto> GetProductByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/product/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProductDto>();
    }
}