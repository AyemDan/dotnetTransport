using Transport.Domain.Entities.MongoDB;
using Transport.Domain.Interfaces.MongoDB;

namespace Transport.Application.Services
{
    public class PaymentLogService
    {
        private readonly IMongoRepository<PaymentLog> _repository;

        public PaymentLogService(IMongoRepository<PaymentLog> repository)
        {
            _repository = repository;
        }

        public async Task<PaymentLog?> GetPaymentLogByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<PaymentLog>> GetAllPaymentLogsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<PaymentLog>> GetPaymentLogsByUserIdAsync(Guid userId)
        {
            return await _repository.FindAsync(p => p.UserId == userId);
        }

        public async Task AddPaymentLogAsync(PaymentLog paymentLog)
        {
            await _repository.AddAsync(paymentLog);
        }

        public async Task UpdatePaymentLogAsync(string id, PaymentLog paymentLog)
        {
            await _repository.UpdateAsync(id, paymentLog);
        }

        public async Task DeletePaymentLogAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
