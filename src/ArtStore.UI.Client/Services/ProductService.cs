using System.Net.Http.Json;

namespace ArtStore.UI.Client.Services;

public class ProductService
{
    private readonly HttpClient _httpClient;
    public ProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<List<ProductModel>> GetProductsAsync()
    {
        var response = await _httpClient.GetAsync("api/product/get-all-products");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<ProductModel>>();
    }
    public async Task<ProductModel> GetProductByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/product/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProductModel>();
    }
}