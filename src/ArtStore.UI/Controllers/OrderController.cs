using ArtStore.Application.Features.Order.Queries;
using ArtStore.Shared.DTOs.Order;
using ArtStore.Shared.DTOs.Order.Commands;
using ArtStore.Shared.Interfaces.Query;

namespace ArtStore.UI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase   
{
    private readonly ICommandHandler<CreateOrderCommand, Result<string>> _createOrderCommandHandler;
    private readonly ICommandHandler<UpdateOrderStatusCommand, Result> _updateOrderStatusCommandHandler;
    private readonly IQueryHandler<GetOrdersQuery, IEnumerable<OrderDto>> _getAllOrdersQueryHandler;
        
    public OrderController(
        ICommandHandler<CreateOrderCommand, Result<string>> createOrderCommandHandler,
        ICommandHandler<UpdateOrderStatusCommand, Result> updateOrderStatusCommandHandler,
        IQueryHandler<GetOrdersQuery, IEnumerable<OrderDto>> getAllOrdersQueryHandler)
    {
        _createOrderCommandHandler = createOrderCommandHandler;
        _updateOrderStatusCommandHandler = updateOrderStatusCommandHandler;
        _getAllOrdersQueryHandler = getAllOrdersQueryHandler;
    }

    [HttpPost("create-order")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var result = await _createOrderCommandHandler.Handle(command, CancellationToken.None);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result);
    }

    [HttpPost("update-order-status")]
    public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusCommand command)
    {
        var result = await _updateOrderStatusCommandHandler.Handle(command, CancellationToken.None);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result);
    }

    [HttpGet("get-orders")]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _getAllOrdersQueryHandler.Handle(new GetOrdersQuery(), CancellationToken.None);
        return Ok(orders);
    }
}
