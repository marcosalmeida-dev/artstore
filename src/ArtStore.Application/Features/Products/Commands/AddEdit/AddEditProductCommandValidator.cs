// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ArtStore.Shared.DTOs.Product.Commands;

namespace ArtStore.Application.Features.Products.Commands.AddEdit;

public class AddEditProductCommandValidator : AbstractValidator<AddEditProductCommand>
{
    public AddEditProductCommandValidator()
    {
        RuleFor(v => v.Name)
            .MaximumLength(256)
            .NotEmpty();
        RuleFor(v => v.Unit)
            .MaximumLength(2)
            .NotEmpty();
        RuleFor(v => v.Brand)
            .MaximumLength(30)
            .NotEmpty();
        RuleFor(v => v.Price)
            .GreaterThanOrEqualTo(0);
        RuleFor(v => v.Description)
            .MaximumLength(1024);
        RuleFor(v => v.CategoryId)
            .GreaterThan(0).WithMessage("Please select a category.");
        RuleFor(v => v.ProductCode)
            .NotEmpty().WithMessage("Product code is required.")
            .MaximumLength(50).WithMessage("Product code must not exceed 50 characters.")
            .Matches("^[A-Z0-9-]+$").WithMessage("Product code must contain only uppercase letters, numbers, and hyphens.");
    }
}