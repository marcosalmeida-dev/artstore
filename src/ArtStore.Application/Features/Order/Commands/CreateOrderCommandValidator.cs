using ArtStore.Shared.DTOs.Order.Commands;

namespace ArtStore.Application.Features.Order.Commands;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerEmail)
            .NotEmpty().WithMessage("Customer email is required.")
            .MaximumLength(200).WithMessage("Customer email must not exceed 200 characters.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Customer name is required.")
            .MaximumLength(200).WithMessage("Customer name must not exceed 200 characters.");

        RuleFor(x => x.CustomerPhone)
            .MaximumLength(20).WithMessage("Customer phone must not exceed 20 characters.");

        RuleFor(x => x.ShippingAddress)
            .NotEmpty().WithMessage("Shipping address is required.")
            .MaximumLength(500).WithMessage("Shipping address must not exceed 500 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters.");

        RuleFor(x => x.TotalAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Total amount must be greater than or equal to 0.")
            .PrecisionScale(2, 18, true).WithMessage("Total amount must have up to 18 digits in total, with 2 decimal places.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid order status.");
    }
}