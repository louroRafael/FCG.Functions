using FCG.Functions.Interfaces;
using FCG.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using System.Text.Json;

namespace FCG.Functions.Functions;

public class ProcessOrderFunction
{
    private readonly IAppLogger<ProcessOrderFunction> _logger;
    private readonly IPaymentClient _paymentClient;

    public ProcessOrderFunction(
        IAppLogger<ProcessOrderFunction> logger,
        IPaymentClient paymentClient)
    {
        _logger = logger;
        _paymentClient = paymentClient;
    }


    [Function(nameof(ProcessOrderFunction))]
    public async Task Run(
        [ServiceBusTrigger("compras-realizadas", Connection = "SERVICEBUS_CONNECTION")]
        string message,
        FunctionContext context,
        CancellationToken ct)
    {
        Order newOrder;

        try
        {
            newOrder = JsonSerializer.Deserialize<Order>(
                message,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? throw new InvalidOperationException("Erro ao serializar pedido");

            var request = new CreatePaymentRequest(newOrder.OrderId, newOrder.TotalAmount, newOrder.PaymentMethod);

            var paymentResponse = await _paymentClient.CreatePaymentAsync(request, ct);

            if (!paymentResponse.Success)
                throw new InvalidOperationException("Erro ao criar pagamento");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao criar pagamento");
            throw;
        }
    }
}
