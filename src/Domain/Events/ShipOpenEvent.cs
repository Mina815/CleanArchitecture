using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Domain.Events;

public class ShipOpenEvent : BaseEvent
{
    public Shop Shop { get; }
    public ShipOpenEvent(Shop shop)
    {
        Shop = shop;
    }
}
