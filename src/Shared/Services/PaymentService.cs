using MongoDB.Driver;
using Transport.Shared.Entities.MongoDB;
using Transport.Shared.Interfaces;
using MongoDB.Bson;

namespace Transport.Shared.Services;

public class PaymentService : IPaymentService
{
    private readonly IMongoRepository<PaymentLog> _paymentRepository;

    public PaymentService(IMongoRepository<PaymentLog> paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<PaymentLog> ProcessPaymentAsync(PaymentLog payment)
    {
        payment.Id = ObjectId.GenerateNewId();
        payment.CreatedAt = DateTime.UtcNow;
        payment.Status = "Completed";
        
        await _paymentRepository.AddAsync(payment);
        return payment;
    }

    public async Task<bool> ReversePaymentAsync(string paymentId, string reason)
    {
        try
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null || payment.Status != "Completed")
            {
                return false;
            }

            payment.Status = "Reversed";
            payment.Type = "Reversal";
            await _paymentRepository.UpdateAsync(paymentId, payment);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<PaymentLog?> GetPaymentByIdAsync(string paymentId)
    {
        return await _paymentRepository.GetByIdAsync(paymentId);
    }

    public async Task<IEnumerable<PaymentLog>> GetUserPaymentsAsync(Guid userId, string? status = null)
    {
        var payments = await _paymentRepository.FindAsync(p => p.UserId == userId);
        
        if (!string.IsNullOrEmpty(status))
        {
            payments = payments.Where(p => p.Status == status);
        }

        return payments.OrderByDescending(p => p.CreatedAt);
    }

    public async Task<object> GetUserPaymentAnalyticsAsync(Guid userId)
    {
        var payments = await GetUserPaymentsAsync(userId);
        var paymentsList = payments.ToList();

        return new
        {
            TotalPayments = paymentsList.Count,
            TotalAmount = paymentsList.Where(p => p.Status == "Completed").Sum(p => p.Amount),
            CompletedPayments = paymentsList.Count(p => p.Status == "Completed"),
            ReversedPayments = paymentsList.Count(p => p.Status == "Reversed"),
            PendingPayments = paymentsList.Count(p => p.Status == "Pending"),
            LastPaymentDate = paymentsList.OrderByDescending(p => p.CreatedAt).FirstOrDefault()?.CreatedAt,
        };
    }
} 