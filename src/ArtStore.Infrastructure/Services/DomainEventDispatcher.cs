using ArtStore.Domain.Common.Events.Dispatcher;

namespace ArtStore.Infrastructure.Services;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public DomainEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task PublishAsync(object domainEvent, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var handlers = scope.ServiceProvider.GetServices<IDomainEventHandler>();

        foreach (var handler in handlers)
        {
            if (handler.CanHandle(domainEvent))
            {
                await handler.HandleAsync(domainEvent, cancellationToken);
            }
        }
    }
}