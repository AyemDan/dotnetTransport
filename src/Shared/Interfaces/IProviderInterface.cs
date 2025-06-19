using Transport.Shared.Entities;

namespace Transport.Shared.Interfaces;

public interface IProviderInterface
{
    Task<Provider?> GetProviderByIdAsync(Guid id);
    Task<Provider?> GetProviderByEmailAsync(string email);
    Task<IEnumerable<Provider>> GetAllProvidersAsync();
    Task<IEnumerable<Provider>> GetProvidersByOrganizationAsync(Guid organizationId);
    Task AddProviderAsync(Provider provider);
    Task UpdateProviderAsync(Provider provider);
    Task DeleteProviderAsync(Guid id);
} 