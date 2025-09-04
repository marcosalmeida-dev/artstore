// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

//using ArtStore.Domain.Common;

//namespace ArtStore.Application.Common.EventHandlers;
//public class CreatedEventHandler<T> : INotificationHandler<DomainEventNotification<CreatedEvent<T>>> where T : IHasDomainEvent
//{
//    private readonly ILogger<CreatedEventHandler<T>> _logger;

//    public CreatedEventHandler(ILogger<CreatedEventHandler<T>> logger)
//    {
//        _logger = logger;
//    }
//    public Task Handle(DomainEventNotification<CreatedEvent<T>> notification, CancellationToken cancellationToken)
//    {
//        var domainEvent = notification.BaseDomainEvent;
//        _logger.LogInformation("Domain Event: {BaseDomainEvent}", domainEvent.GetType().Name);
//        return Task.CompletedTask;
//    }
//}