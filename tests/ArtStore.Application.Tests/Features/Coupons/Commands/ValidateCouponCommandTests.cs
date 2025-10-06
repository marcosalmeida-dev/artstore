using ArtStore.Application.Features.Coupons.Commands.Validate;
using ArtStore.Application.Tests.Fixtures;
using ArtStore.Shared.DTOs.Coupon.Commands;
using FluentAssertions;

namespace ArtStore.Application.Tests.Features.Coupons.Commands;

public class ValidateCouponCommandTests : TestBase
{
    public ValidateCouponCommandTests(ApplicationDbContextFixture dbFixture) : base(dbFixture)
    {
    }

    [Fact]
    public async Task Handle_WhenValidCouponAndOrderAmount_ShouldReturnSuccess()
    {
        // Arrange
        var context = GetDbContext();
        var handler = new ValidateCouponCommandHandler(context);

        var command = new ValidateCouponCommand
        {
            Code = "SAVE10",
            OrderTotal = 100m
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.DiscountAmount.Should().Be(10m); // 10% of 100
    }

    [Fact]
    public async Task Handle_WhenCouponNotFound_ShouldReturnInvalid()
    {
        // Arrange
        var context = GetDbContext();
        var handler = new ValidateCouponCommandHandler(context);

        var command = new ValidateCouponCommand
        {
            Code = "INVALID_CODE",
            OrderTotal = 100m
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_WhenOrderAmountBelowMinimum_ShouldReturnInvalid()
    {
        // Arrange
        var context = GetDbContext();
        var handler = new ValidateCouponCommandHandler(context);

        var command = new ValidateCouponCommand
        {
            Code = "SAVE10",
            OrderTotal = 30m // Below minimum of 50
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("Minimum");
    }

    [Fact]
    public async Task Handle_WhenCouponInactive_ShouldReturnInvalid()
    {
        // Arrange
        var context = GetDbContext();

        // Deactivate the coupon
        var coupon = await context.Coupons.FindAsync(1);
        coupon!.IsActive = false;
        await context.SaveChangesAsync();

        var handler = new ValidateCouponCommandHandler(context);

        var command = new ValidateCouponCommand
        {
            Code = "SAVE10",
            OrderTotal = 100m
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();

        // Restore coupon state
        coupon.IsActive = true;
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_WhenCouponExpired_ShouldReturnInvalid()
    {
        // Arrange
        var context = GetDbContext();

        // Set coupon as expired
        var coupon = await context.Coupons.FindAsync(1);
        coupon!.EndDate = DateTime.UtcNow.AddDays(-1);
        await context.SaveChangesAsync();

        var handler = new ValidateCouponCommandHandler(context);

        var command = new ValidateCouponCommand
        {
            Code = "SAVE10",
            OrderTotal = 100m
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("expired");

        // Restore coupon state
        coupon.EndDate = DateTime.UtcNow.AddDays(20);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_WhenPercentageDiscount_ShouldCalculateCorrectly()
    {
        // Arrange
        var context = GetDbContext();
        var handler = new ValidateCouponCommandHandler(context);

        var command = new ValidateCouponCommand
        {
            Code = "SAVE10",
            OrderTotal = 200m
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.DiscountAmount.Should().Be(20m); // 10% of 200
    }
}
