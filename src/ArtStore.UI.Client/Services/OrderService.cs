using System.Net.Http.Json;
using ArtStore.Shared.DTOs;
using ArtStore.Shared.DTOs.Order;
using ArtStore.Shared.DTOs.Order.Commands;

namespace ArtStore.UI.Client.Services;

public class OrderService
{
    private readonly HttpClient _httpClient;
    public OrderService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> CreateOrderAsync(CreateOrderCommand createOrderCommand)
    {
        var response = await _httpClient.PostAsJsonAsync("api/order/create-order", createOrderCommand);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<Result<string>>();

        return result.Data;
    }

    public async Task<bool> UpdateOrderStatusAsync(UpdateOrderStatusCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync("api/order/update-order-status", command);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Result>();
        return result.Succeeded;
    }

    public async Task<List<OrderDto>> GetOrdersAsync()
    {
        var response = await _httpClient.GetAsync("api/order/get-orders");
        response.EnsureSuccessStatusCode();
        var orders = await response.Content.ReadFromJsonAsync<List<OrderDto>>();
        return orders ?? new List<OrderDto>();
    }
}