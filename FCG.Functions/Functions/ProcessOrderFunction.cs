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
        [ServiceBusTrigger("order-created", Connection = "SERVICEBUS_CONNECTION")]
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
            ) ?? throw new InvalidOperationException("Erro ao desserializar pedido");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mensagem inválida recebida. Payload: {Message}", message);
            return;
        }

        try
        {
            var request = new CreatePaymentRequest(newOrder.OrderId, newOrder.TotalAmount, newOrder.PaymentMethod);

            var result = await _paymentClient.CreatePaymentAsync(request, ct);

            if (!result.Success)
            {
                _logger.LogWarning(
                    "Falha ao criar pagamento. OrderId={OrderId}. Erros={Errors}",
                    newOrder.OrderId,
                    result.Errors
                );

                return;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro de comunicação com Payments API");
            throw;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout ao chamar Payments API");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao processar pagamento");
            throw;
        }
    }
}
