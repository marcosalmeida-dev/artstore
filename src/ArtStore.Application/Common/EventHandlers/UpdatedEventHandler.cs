﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

//using ArtStore.Domain.Common;

//namespace ArtStore.Application.Common.EventHandlers;
//public class UpdatedEventHandler<T> : INotificationHandler<DomainEventNotification<UpdatedEvent<T>>> where T : IHasDomainEvent
//{
//    private readonly ILogger<UpdatedEventHandler<T>> _logger;
//    public UpdatedEventHandler(ILogger<UpdatedEventHandler<T>> logger)
//    {
//        _logger = logger;
//    }
//    public Task Handle(DomainEventNotification<UpdatedEvent<T>> notification, CancellationToken cancellationToken)
//    {
//        var domainEvent = notification.BaseDomainEvent;
//        _logger.LogInformation("Domain Event: {BaseDomainEvent}", domainEvent.GetType().Name);
//        return Task.CompletedTask;
//    }
//}