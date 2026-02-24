using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Domain.Events;

public class ShopCreatedEvent : BaseEvent
{
    public Shop Shop { get; }
    public ShopCreatedEvent(Shop shop)
    {
        Shop = shop;
    }
}
