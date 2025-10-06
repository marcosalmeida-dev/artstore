# ArtStore.Application.Tests

This project contains unit tests for the ArtStore.Application layer using XUnit, Moq, and FluentAssertions.

## Structure

```
ArtStore.Application.Tests/
├── Features/
│   ├── Products/
│   │   ├── Commands/
│   │   │   └── AddEditProductCommandTests.cs
│   │   └── Queries/
│   │       └── GetAllProductsQueryTests.cs
│   ├── Categories/
│   │   ├── Commands/
│   │   │   └── AddEditCategoryCommandTests.cs
│   │   └── Queries/
│   │       └── GetAllCategoriesQueryTests.cs
│   ├── Coupons/
│   │   └── Commands/
│   │       └── ValidateCouponCommandTests.cs
│   └── Orders/
│       └── Queries/
│           └── GetOrdersQueryTests.cs
├── Fixtures/
│   ├── ApplicationDbContextFixture.cs
│   └── TestBase.cs
└── README.md
```

## Technologies Used

- **XUnit** - Testing framework
- **Moq** - Mocking framework for creating mock objects
- **FluentAssertions** - Fluent assertion library for more readable tests
- **Entity Framework Core InMemory** - In-memory database for testing

## Test Fixtures

### ApplicationDbContextFixture
Provides an in-memory database context that is seeded with test data. Each test class can use this fixture to access a consistent database state.

**Seeded Data:**
- **Categories:** Electronics, Clothing
- **Products:** Laptop, T-Shirt
- **Coupons:** SAVE10 (10% discount)

### TestBase
Base class for all test classes providing:
- Access to the database fixture
- Mock objects for common dependencies (HybridCache, IBlobStorageService, IDateTime, etc.)
- Helper methods for database operations

## Running Tests

### Run all tests
```bash
dotnet test
```

### Run tests in a specific class
```bash
dotnet test --filter FullyQualifiedName~AddEditProductCommandTests
```

### Run tests with detailed output
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Run tests with code coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## Test Patterns

### Arrange-Act-Assert (AAA)
All tests follow the AAA pattern:
```csharp
[Fact]
public async Task Handle_WhenCreatingNewProduct_ShouldAddProductToDatabase()
{
    // Arrange - Set up test data and dependencies
    var context = GetDbContext();
    var handler = new AddEditProductCommandHandler(context, ...);
    var command = new AddEditProductCommand { ... };

    // Act - Execute the method under test
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert - Verify the expected outcome
    result.Should().NotBeNull();
    result.Succeeded.Should().BeTrue();
}
```

## Test Coverage

### Products
- ✅ Create new product
- ✅ Update existing product
- ✅ Handle non-existent product
- ✅ Cache invalidation
- ✅ Query all products
- ✅ Query product by ID

### Categories
- ✅ Create new category
- ✅ Update existing category
- ✅ Handle non-existent category
- ✅ Validation tests
- ✅ Query all categories

### Coupons
- ✅ Validate coupon
- ✅ Check minimum order amount
- ✅ Check expiration
- ✅ Calculate discounts

### Orders
- ✅ Get all orders
- ✅ Order by date descending
- ✅ Include order details

## Best Practices

1. **Use InMemory Database:** Tests use Entity Framework Core's InMemory provider for fast, isolated tests
2. **Mock External Dependencies:** Services like BlobStorage and Cache are mocked to avoid external dependencies
3. **Clear Test Names:** Test names follow the pattern `MethodName_Scenario_ExpectedBehavior`
4. **FluentAssertions:** Use FluentAssertions for more readable and maintainable assertions
5. **Isolated Tests:** Each test should be independent and not rely on other tests

## Adding New Tests

1. Create a new test class in the appropriate `Features/{FeatureName}/{Commands|Queries}` folder
2. Inherit from `TestBase`
3. Use the constructor to inject `ApplicationDbContextFixture`
4. Write tests using the AAA pattern
5. Use FluentAssertions for assertions

Example:
```csharp
public class MyNewFeatureTests : TestBase
{
    public MyNewFeatureTests(ApplicationDbContextFixture dbFixture) : base(dbFixture)
    {
    }

    [Fact]
    public async Task MyTest_Scenario_ExpectedResult()
    {
        // Arrange
        var context = GetDbContext();

        // Act

        // Assert
    }
}
```

## Continuous Integration

These tests are designed to run in CI/CD pipelines. Make sure to:
- Keep tests fast (avoid Thread.Sleep, use async/await properly)
- Ensure tests are deterministic (no random data, fixed dates for time-sensitive tests)
- Clean up resources properly (fixtures implement IDisposable)

## Future Improvements

- [ ] Add integration tests
- [ ] Add performance tests
- [ ] Increase code coverage to 80%+
- [ ] Add tests for event handlers
- [ ] Add tests for validators
- [ ] Add mutation testing
