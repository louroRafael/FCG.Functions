using FCG.Functions.Enums;
using FCG.Functions.Interfaces;
using FCG.Functions.Models;
using FCG.Functions.Models.Common;
using System.Net.Http.Json;

namespace FCG.Functions.Services;

public class PaymentClient : IPaymentClient
{
    private readonly HttpClient _httpClient;

    public PaymentClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result<PaymentResponse>> CreatePaymentAsync(CreatePaymentRequest request, CancellationToken ct)
    {
        using var response = await _httpClient.PostAsJsonAsync("api/payments", request, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);

            return Result<PaymentResponse>.Fail(
                new Error(
                    ErrorType.ExternalService,
                    "PAYMENT_API_ERROR",
                    $"Erro ao chamar Payment API: {error}"
                )
            );
        }

        var result = await response.Content.ReadFromJsonAsync<PaymentResponse>(cancellationToken: ct);

        return result != null
        ? Result<PaymentResponse>.Ok(result)
        : Result<PaymentResponse>.Fail(
            new Error(ErrorType.NotFound, "NULL_RESPONSE", "Resposta do pagamento nula")
        );
    }
}
