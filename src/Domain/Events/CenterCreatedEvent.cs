using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.Events;

public class CenterCreatedEvent : BaseEvent
{
    public CenterCreatedEvent(BeautyCenter center)
    {
        Center = center;
    }

    public BeautyCenter Center { get; }
}
