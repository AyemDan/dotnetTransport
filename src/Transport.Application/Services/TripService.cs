using Transport.Domain.Entities.MongoDB;
using Transport.Domain.Interfaces.MongoDB;


namespace Transport.Application.Services
{
    public class TripService
    {
        private readonly IMongoRepository<Trip> _repository;

        public TripService(IMongoRepository<Trip> repository)
        {
            _repository = repository;
        }

        public async Task<Trip?> GetTripByIdAsync(string tripId)
        {
            return await _repository.GetByIdAsync(tripId);
        }

        public async Task<IEnumerable<Trip>> GetAllTripsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<Trip>> GetTripsByUserIdAsync(Guid userId)
        {
            return await _repository.FindAsync(t => t.UserId == userId);
        }

        public async Task AddTripAsync(Trip trip)
        {
            await _repository.AddAsync(trip);
        }

        public async Task UpdateTripAsync(string tripId, Trip trip)
        {
            await _repository.UpdateAsync(tripId, trip);
        }

        public async Task DeleteTripAsync(string tripId)
        {
            await _repository.DeleteAsync(tripId);
        }
    }
}
