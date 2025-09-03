//using ArtStore.Application.Features.Categories.Caching;

//namespace ArtStore.Application.Features.Categories.EventHandlers;

//public class CategoryCreatedEventHandler : INotificationHandler<CreatedEvent<Category>>
//{
//    private readonly ILogger<CategoryCreatedEventHandler> _logger;

//    public CategoryCreatedEventHandler(ILogger<CategoryCreatedEventHandler> logger)
//    {
//        _logger = logger;
//    }

//    public async Task Handle(CreatedEvent<Category> notification, CancellationToken cancellationToken)
//    {
//        _logger.LogInformation("Category created: {CategoryName} with ID: {CategoryId}", 
//            notification.Entity.Name, notification.Entity.Id);
//        
//        CategoryCacheKey.Refresh();
//        
//        await Task.CompletedTask;
//    }
//}