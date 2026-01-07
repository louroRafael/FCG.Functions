using FCG.Functions.Enums;

namespace FCG.Functions.Models;

public record CreatePaymentRequest(Guid OrderId, decimal TotalAmount, PaymentMethod PaymentMethod);
