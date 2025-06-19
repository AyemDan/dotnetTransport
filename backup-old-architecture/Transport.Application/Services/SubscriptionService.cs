using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transport.Domain.Entities.MongoDB;
using Transport.Domain.Interfaces.MongoDB;

namespace Transport.Application.Services
{
    public class SubscriptionService
    {
        private readonly IMongoRepository<Subscription> _subscriptionRepo;
        public SubscriptionService(IMongoRepository<Subscription> subscriptionRepo)
        {
            _subscriptionRepo = subscriptionRepo;
        }

        public async Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync()
        {
            return await _subscriptionRepo.GetAllAsync();
        }

        public async Task<Subscription?> GetSubscriptionByIdAsync(string id)
        {
            return await _subscriptionRepo.GetByIdAsync(id);
        }

        public async Task AddSubscriptionAsync(Subscription subscription)
        {
            await _subscriptionRepo.AddAsync(subscription);
        }

        public async Task UpdateSubscriptionAsync(string id, Subscription subscription)
        {
            await _subscriptionRepo.UpdateAsync(id, subscription);
        }

        public async Task DeleteSubscriptionAsync(string id)
        {
            await _subscriptionRepo.DeleteAsync(id);
        }
    }
} 