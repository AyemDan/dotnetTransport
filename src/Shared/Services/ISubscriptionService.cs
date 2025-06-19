using Transport.Shared.Entities.MongoDB;
using Transport.Shared.Enums;

namespace Transport.Shared.Services;

public interface ISubscriptionService
{
    Task<Subscription?> GetSubscriptionAsync(Guid subscriptionId);
    Task<Subscription> CreateSubscriptionAsync(Subscription subscription);
    Task<bool> UpdateSubscriptionAsync(Subscription subscription);
    Task<bool> CancelSubscriptionAsync(Guid subscriptionId);
    Task<IEnumerable<Subscription>> GetSubscriptionsByUserIdAsync(Guid userId);
    Task<bool> ActivateSubscriptionAsync(Guid subscriptionId);
    Task<bool> DeactivateSubscriptionAsync(Guid subscriptionId);
    Task<SubscriptionStatus> GetSubscriptionStatusAsync(Guid subscriptionId);
} 