using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transport.Domain.Entities.MongoDB;
using Transport.Domain.Interfaces.MongoDB;

namespace Transport.Application.Services
{
    public class RouteService
    {
        private readonly IMongoRepository<Route> _routeRepo;
        public RouteService(IMongoRepository<Route> routeRepo)
        {
            _routeRepo = routeRepo;
        }

        public async Task<IEnumerable<Route>> GetAllRoutesAsync()
        {
            return await _routeRepo.GetAllAsync();
        }

        public async Task<Route?> GetRouteByIdAsync(string id)
        {
            return await _routeRepo.GetByIdAsync(id);
        }

        public async Task AddRouteAsync(Route route)
        {
            await _routeRepo.AddAsync(route);
        }

        public async Task UpdateRouteAsync(string id, Route route)
        {
            await _routeRepo.UpdateAsync(id, route);
        }

        public async Task DeleteRouteAsync(string id)
        {
            await _routeRepo.DeleteAsync(id);
        }
    }
} 