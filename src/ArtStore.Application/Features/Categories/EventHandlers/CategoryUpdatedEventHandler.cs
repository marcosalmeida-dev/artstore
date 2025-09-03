//using ArtStore.Application.Features.Categories.Caching;

//namespace ArtStore.Application.Features.Categories.EventHandlers;

//public class CategoryUpdatedEventHandler : INotificationHandler<UpdatedEvent<Category>>
//{
//    private readonly ILogger<CategoryUpdatedEventHandler> _logger;

//    public CategoryUpdatedEventHandler(ILogger<CategoryUpdatedEventHandler> logger)
//    {
//        _logger = logger;
//    }

//    public async Task Handle(UpdatedEvent<Category> notification, CancellationToken cancellationToken)
//    {
//        _logger.LogInformation("Category updated: {CategoryName} with ID: {CategoryId}", 
//            notification.Entity.Name, notification.Entity.Id);
//        
//        CategoryCacheKey.Refresh();
//        
//        await Task.CompletedTask;
//    }
//}