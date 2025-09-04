
namespace ArtStore.Application.Hubs;

public interface IOrderManagementHub
{
    public const string Url = "/orderManagementHub";

    public const string OrderCreated = "OrderCreated";
    public const string OrderStatusChanged = "OrderStatusChanged";

    Task OnConnectedAsync();
}