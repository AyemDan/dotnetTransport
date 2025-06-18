using Transport.Domain.Entities.MongoDB;
using Transport.Domain.Interfaces.MongoDB;


namespace Transport.Application.Services
{
    public class PaymentInfoService
    {
        private readonly IMongoRepository<PaymentInfo> _repository;

        public PaymentInfoService(IMongoRepository<PaymentInfo> repository)
        {
            _repository = repository;
        }

        public async Task<PaymentInfo?> GetPaymentInfoByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<PaymentInfo>> GetAllPaymentInfosAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<PaymentInfo?> GetPaymentInfoByUserIdAsync(Guid userId)
        {
            var results = await _repository.FindAsync(p => p.UserId == userId);
            return results.FirstOrDefault(); // Assuming one payment info per user
        }

        public async Task AddPaymentInfoAsync(PaymentInfo paymentInfo)
        {
            await _repository.AddAsync(paymentInfo);
        }

        public async Task UpdatePaymentInfoAsync(string id, PaymentInfo paymentInfo)
        {
            await _repository.UpdateAsync(id, paymentInfo);
        }

        public async Task DeletePaymentInfoAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
