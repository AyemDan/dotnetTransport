using Microsoft.EntityFrameworkCore;
using Transport.Domain.Entities;
using Transport.Infrastructure.Data;

namespace Transport.Infrastructure.Repositories;

public class ProviderRepository : UserRepository
{
    public ProviderRepository(PostgresDbContext context) : base(context) {}

    public async Task<List<Provider>> GetAllProvidersAsync()
    {
        return await _context.Set<Provider>().ToListAsync();
    }

    public async Task<List<Provider>> GetActiveProvidersAsync()
    {
        return await _context.Set<Provider>().Where(p => p.IsActive).ToListAsync();
    }
}
