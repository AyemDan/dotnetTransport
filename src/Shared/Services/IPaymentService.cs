using Transport.Shared.Entities.MongoDB;

namespace Transport.Shared.Services;

public interface IPaymentService
{
    Task<PaymentLog> ProcessPaymentAsync(PaymentLog payment);
    Task<bool> ReversePaymentAsync(string paymentId, string reason);
    Task<PaymentLog?> GetPaymentByIdAsync(string paymentId);
    Task<IEnumerable<PaymentLog>> GetUserPaymentsAsync(Guid userId, string? status = null);
    Task<object> GetUserPaymentAnalyticsAsync(Guid userId);
} 