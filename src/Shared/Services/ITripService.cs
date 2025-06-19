using Transport.Shared.Entities.MongoDB;
using Transport.Shared.Enums;

namespace Transport.Shared.Services;

public interface ITripService
{
    Task<Trip?> GetTripAsync(Guid tripId);
    Task<Trip> CreateTripAsync(Trip trip);
    Task<bool> UpdateTripAsync(Trip trip);
    Task<bool> CancelTripAsync(Guid tripId);
    Task<IEnumerable<Trip>> GetTripsByRouteIdAsync(Guid routeId);
    Task<IEnumerable<Trip>> GetTripsByDriverIdAsync(Guid driverId);
    Task<IEnumerable<Trip>> GetTripsByOrganizationIdAsync(Guid organizationId);
    Task<bool> StartTripAsync(Guid tripId);
    Task<bool> EndTripAsync(Guid tripId);
    Task<TripStatus> GetTripStatusAsync(Guid tripId);
} 