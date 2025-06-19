using Transport.Domain.Entities.MongoDB;
using Transport.Domain.Interfaces.MongoDB;

namespace Transport.Application.Services
{
    public class UserPreferencesService
    {
        private readonly IMongoRepository<UserPreferences> _repository;

        public UserPreferencesService(IMongoRepository<UserPreferences> repository)
        {
            _repository = repository;
        }

        public async Task<UserPreferences?> GetPreferencesAsync(Guid userId)
        {
            var results = await _repository.FindAsync(p => p.UserId == userId);
            return results.FirstOrDefault(); // Returns null if not found
        }

        public async Task AddOrUpdatePreferencesAsync(UserPreferences preferences)
        {
            var existing = await GetPreferencesAsync(preferences.UserId);
            if (existing != null)
            {
                await _repository.UpdateAsync(existing.Id.ToString(), preferences);
            }
            else
            {
                await _repository.AddAsync(preferences);
            }
        }

        public async Task DeletePreferencesAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
