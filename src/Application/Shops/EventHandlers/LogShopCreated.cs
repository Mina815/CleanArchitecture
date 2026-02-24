using System;
using System.Collections.Generic;
using System.Text;
using CleanArchitecture.Application.TodoItems.EventHandlers;
using CleanArchitecture.Domain.Events;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Shops.EventHandlers;

public class LogShopCreated : INotificationHandler<ShopCreatedEvent>
{
    private readonly ILogger<LogShopCreated> _logger;

    public LogShopCreated(ILogger<LogShopCreated> logger)
    {
        _logger = logger;
    }

    public Task Handle(ShopCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
