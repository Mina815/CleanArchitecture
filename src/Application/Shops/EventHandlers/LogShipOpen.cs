using System;
using System.Collections.Generic;
using System.Text;
using CleanArchitecture.Application.TodoItems.EventHandlers;
using CleanArchitecture.Domain.Events;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Shops.EventHandlers;

public class LogShipOpen : INotificationHandler<ShipOpenEvent>
{
    private readonly ILogger<LogShipOpen> _logger;

    public LogShipOpen(ILogger<LogShipOpen> logger)
    {
        _logger = logger;
    }

    public Task Handle(ShipOpenEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
