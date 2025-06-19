using Transport.Shared.Entities.MongoDB;

namespace Transport.Shared.Services;

public interface IRouteService
{
    Task<Route?> GetRouteAsync(Guid routeId);
    Task<Route> CreateRouteAsync(Route route);
    Task<bool> UpdateRouteAsync(Route route);
    Task<bool> DeleteRouteAsync(Guid routeId);
    Task<IEnumerable<Route>> GetRoutesByProviderIdAsync(Guid providerId);
    Task<IEnumerable<Route>> GetRoutesByOrganizationIdAsync(Guid organizationId);
    Task<IEnumerable<Route>> GetActiveRoutesAsync();
    Task<bool> ActivateRouteAsync(Guid routeId);
    Task<bool> DeactivateRouteAsync(Guid routeId);
} 