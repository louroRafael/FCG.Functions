using FCG.Functions.Models;
using FCG.Functions.Models.Common;

namespace FCG.Functions.Interfaces;

public interface IPaymentClient
{
    Task<Result<PaymentResponse>> CreatePaymentAsync(CreatePaymentRequest request, CancellationToken ct);
}
