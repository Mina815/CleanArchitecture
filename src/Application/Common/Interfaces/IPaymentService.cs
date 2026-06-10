using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Application.Common.Interfaces;

public record PaymentInitiationResult(string? PaymentUrl, string? ProviderReference, string? TransactionId);

public interface IPaymentService
{
    PaymentProvider Provider { get; }
    Task<PaymentInitiationResult> InitiatePaymentAsync(int bookingId, decimal amount, string currency, CancellationToken cancellationToken = default);
    Task<bool> VerifyPaymentAsync(string providerReference, CancellationToken cancellationToken = default);
}

public interface IPaymentServiceFactory
{
    IPaymentService GetService(PaymentProvider provider);
}
