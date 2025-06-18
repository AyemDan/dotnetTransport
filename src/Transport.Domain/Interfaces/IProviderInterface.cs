using Transport.Domain.Entities;

namespace Transport.Domain.Interfaces;

public interface IProviderRepository : IUserRepository
{
    Task<List<Provider>> GetAllProvidersAsync();
    Task<List<Provider>> GetActiveProvidersAsync();
}
