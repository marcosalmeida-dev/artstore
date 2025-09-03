using ArtStore.Shared.DTOs.Category.Commands;
using FluentValidation;

namespace ArtStore.Application.Features.Categories.Commands.AddEdit;

public class AddEditCategoryCommandValidator : AbstractValidator<AddEditCategoryCommand>
{
    public AddEditCategoryCommandValidator()
    {
        RuleFor(v => v.Name)
            .MaximumLength(100)
            .NotEmpty();
        RuleFor(v => v.Description)
            .MaximumLength(500);
    }
}