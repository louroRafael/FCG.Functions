using FCG.Functions.Enums;

namespace FCG.Functions.Models;

public record PaymentResponse(Guid Id, Guid OrderId, decimal TotalAmount, PaymentMethod PaymentMethod, PaymentStatus PaymentStatus);
