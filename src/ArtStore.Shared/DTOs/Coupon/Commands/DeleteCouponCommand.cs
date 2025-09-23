using ArtStore.Shared.Interfaces.Command;

namespace ArtStore.Shared.DTOs.Coupon.Commands;

public class DeleteCouponCommand : ICommand<Result<int>>
{
    public int Id { get; set; }
}