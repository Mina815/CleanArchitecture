using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Services.Payments;

public class PayPalPaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PayPalPaymentService> _logger;

    public PayPalPaymentService(IConfiguration configuration, ILogger<PayPalPaymentService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public PaymentProvider Provider => PaymentProvider.PayPal;

    public Task<PaymentInitiationResult> InitiatePaymentAsync(int bookingId, decimal amount, string currency, CancellationToken cancellationToken = default)
    {
        var clientId = _configuration["Payments:PayPal:ClientId"];
        var transactionId = $"PP-{bookingId}-{Guid.NewGuid():N}"[..24];
        var paymentUrl = string.IsNullOrEmpty(clientId)
            ? null
            : $"https://www.sandbox.paypal.com/checkoutnow?token=stub-{transactionId}";

        _logger.LogInformation("PayPal payment initiated for booking {BookingId}, amount {Amount} {Currency}", bookingId, amount, currency);

        return Task.FromResult(new PaymentInitiationResult(paymentUrl, transactionId, transactionId));
    }

    public Task<bool> VerifyPaymentAsync(string providerReference, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Verifying PayPal payment {Reference}", providerReference);
        return Task.FromResult(!string.IsNullOrEmpty(providerReference));
    }
}
