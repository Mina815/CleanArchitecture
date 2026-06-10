using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Services.Payments;

public class PaymobPaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PaymobPaymentService> _logger;

    public PaymobPaymentService(IConfiguration configuration, ILogger<PaymobPaymentService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public PaymentProvider Provider => PaymentProvider.Paymob;

    public Task<PaymentInitiationResult> InitiatePaymentAsync(int bookingId, decimal amount, string currency, CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["Payments:Paymob:ApiKey"];
        var transactionId = $"PM-{bookingId}-{Guid.NewGuid():N}"[..24];
        var paymentUrl = string.IsNullOrEmpty(apiKey)
            ? null
            : $"https://accept.paymob.com/api/acceptance/iframes/{_configuration["Payments:Paymob:IframeId"]}?payment_token=stub-{transactionId}";

        _logger.LogInformation("Paymob payment initiated for booking {BookingId}, amount {Amount} {Currency}", bookingId, amount, currency);

        return Task.FromResult(new PaymentInitiationResult(paymentUrl, transactionId, transactionId));
    }

    public Task<bool> VerifyPaymentAsync(string providerReference, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Verifying Paymob payment {Reference}", providerReference);
        return Task.FromResult(!string.IsNullOrEmpty(providerReference));
    }
}
