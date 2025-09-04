using Microsoft.AspNetCore.SignalR;

namespace ArtStore.Application.Hubs;

public class OrderManagementHub : Hub<IOrderManagementHub>
{
    public override Task OnConnectedAsync()
    {
        // Logic for when a client connects to the hub
        return base.OnConnectedAsync();
    }
}