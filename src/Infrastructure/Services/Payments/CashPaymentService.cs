using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Infrastructure.Services.Payments;

public class CashPaymentService : IPaymentService
{
    public PaymentProvider Provider => PaymentProvider.Cash;

    public Task<PaymentInitiationResult> InitiatePaymentAsync(int bookingId, decimal amount, string currency, CancellationToken cancellationToken = default)
    {
        var transactionId = $"CASH-{bookingId}-{DateTime.UtcNow:yyyyMMddHHmmss}";
        return Task.FromResult(new PaymentInitiationResult(null, transactionId, transactionId));
    }

    public Task<bool> VerifyPaymentAsync(string providerReference, CancellationToken cancellationToken = default)
        => Task.FromResult(true);
}
