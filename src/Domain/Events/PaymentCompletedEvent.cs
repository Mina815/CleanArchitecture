using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.Events;

public class PaymentCompletedEvent : BaseEvent
{
    public PaymentCompletedEvent(Payment payment)
    {
        Payment = payment;
    }

    public Payment Payment { get; }
}
