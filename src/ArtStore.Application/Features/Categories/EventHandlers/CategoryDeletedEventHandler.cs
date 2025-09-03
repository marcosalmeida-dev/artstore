//using ArtStore.Application.Features.Categories.Caching;

//namespace ArtStore.Application.Features.Categories.EventHandlers;

//public class CategoryDeletedEventHandler : INotificationHandler<DeletedEvent<Category>>
//{
//    private readonly ILogger<CategoryDeletedEventHandler> _logger;

//    public CategoryDeletedEventHandler(ILogger<CategoryDeletedEventHandler> logger)
//    {
//        _logger = logger;
//    }

//    public async Task Handle(DeletedEvent<Category> notification, CancellationToken cancellationToken)
//    {
//        _logger.LogInformation("Category deleted: {CategoryName} with ID: {CategoryId}", 
//            notification.Entity.Name, notification.Entity.Id);
//        
//        CategoryCacheKey.Refresh();
//        
//        await Task.CompletedToken;
//    }
//}