using ArtStore.Application.Features.Categories.Commands.AddEdit;
using ArtStore.Application.Tests.Fixtures;
using ArtStore.Shared.DTOs.Category;
using ArtStore.Shared.DTOs.Category.Commands;
using FluentAssertions;

namespace ArtStore.Application.Tests.Features.Categories.Commands;

public class AddEditCategoryCommandTests : TestBase
{
    public AddEditCategoryCommandTests(ApplicationDbContextFixture dbFixture) : base(dbFixture)
    {
    }

    [Fact]
    public async Task Handle_WhenCreatingNewCategory_ShouldAddCategoryToDatabase()
    {
        // Arrange
        var context = GetDbContext();
        var handler = new AddEditCategoryCommandHandler(context);

        var command = new AddEditCategoryCommand
        {
            Id = 0,
            Name = "New Category",
            Description = "Test category description",
            IsActive = true
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Data.Should().BeGreaterThan(0);

        var category = await context.Categories.FindAsync(result.Data);
        category.Should().NotBeNull();
        category!.Name.Should().Be("New Category");
        category.Description.Should().Be("Test category description");
        category.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenUpdatingExistingCategory_ShouldUpdateCategoryInDatabase()
    {
        // Arrange
        var context = GetDbContext();
        var handler = new AddEditCategoryCommandHandler(context);

        var command = new AddEditCategoryCommand
        {
            Id = 1, // Existing category from seed
            Name = "Updated Electronics",
            Description = "Updated description",
            IsActive = false
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();

        var category = await context.Categories.FindAsync(1);
        category.Should().NotBeNull();
        category!.Name.Should().Be("Updated Electronics");
        category.Description.Should().Be("Updated description");
        category.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenCategoryIdDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var context = GetDbContext();
        var handler = new AddEditCategoryCommandHandler(context);

        var command = new AddEditCategoryCommand
        {
            Id = 9999,
            Name = "Non-existent Category",
            Description = "Test",
            IsActive = true
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain(msg => msg.Contains("not found"));
    }

    [Fact]
    public async Task Validate_WhenNameIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new AddEditCategoryCommandValidator();
        var command = new AddEditCategoryCommand
        {
            Id = 0,
            Name = "", // Empty name
            Description = "Test"
        };

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Name));
    }

    [Fact]
    public async Task Validate_WhenNameIsTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new AddEditCategoryCommandValidator();
        var command = new AddEditCategoryCommand
        {
            Id = 0,
            Name = new string('A', 101), // Exceeds max length
            Description = "Test"
        };

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Name));
    }

    [Fact]
    public async Task Validate_WhenValidCommand_ShouldPass()
    {
        // Arrange
        var validator = new AddEditCategoryCommandValidator();
        var command = new AddEditCategoryCommand
        {
            Id = 0,
            Name = "Valid Category",
            Description = "Valid description"
        };

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
