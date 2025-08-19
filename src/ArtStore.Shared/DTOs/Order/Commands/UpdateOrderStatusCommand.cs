using ArtStore.Shared.Interfaces.Command;
using ArtStore.Shared.Models.Enums;

namespace ArtStore.Shared.DTOs.Order.Commands;

public class UpdateOrderStatusCommand : ICommand<Result>
{
    public int OrderId { get; set; }
    public OrderStatus Status { get; set; }
    public string? Notes { get; set; }
}