using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Infrastructure.Services.Payments;

public class PaymentServiceFactory : IPaymentServiceFactory
{
    private readonly IEnumerable<IPaymentService> _services;

    public PaymentServiceFactory(IEnumerable<IPaymentService> services)
    {
        _services = services;
    }

    public IPaymentService GetService(PaymentProvider provider)
        => _services.FirstOrDefault(s => s.Provider == provider)
           ?? throw new NotSupportedException($"Payment provider {provider} is not supported.");
}
