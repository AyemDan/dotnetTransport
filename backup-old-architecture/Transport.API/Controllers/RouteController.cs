using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Transport.Application.Services;
using DomainRoute = Transport.Domain.Entities.MongoDB.Route;
using Transport.API.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Transport.API.Controllers
{
    [ApiController]
    [Route("api/route")]
    [Authorize(Roles = "OrganizationAdmin")]
    public class RouteController : ControllerBase
    {
        private readonly RouteService _routeService;
        public RouteController(RouteService routeService)
        {
            _routeService = routeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoutes()
        {
            var routes = await _routeService.GetAllRoutesAsync();
            return Ok(routes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRouteById(string id)
        {
            var route = await _routeService.GetRouteByIdAsync(id);
            if (route == null) return NotFound();
            return Ok(route);
        }

        [HttpPost]
        public async Task<IActionResult> AddRoute([FromBody] RouteDto dto)
        {
            var route = new DomainRoute
            {
                Name = dto.Name,
                Stops = dto.Stops?.ConvertAll(s => new Transport.Domain.Entities.MongoDB.RouteStop {
                    Name = s.Name,
                    Order = s.Order,
                    PriceFromStart = s.PriceFromStart
                }),
                Schedule = dto.Schedule,
                Price = dto.Price,
                IsActive = dto.IsActive,
                CreatedAt = System.DateTime.UtcNow
            };
            await _routeService.AddRouteAsync(route);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoute(string id, [FromBody] RouteDto dto)
        {
            var route = new DomainRoute
            {
                Name = dto.Name,
                Stops = dto.Stops?.ConvertAll(s => new Transport.Domain.Entities.MongoDB.RouteStop {
                    Name = s.Name,
                    Order = s.Order,
                    PriceFromStart = s.PriceFromStart
                }),
                Schedule = dto.Schedule,
                Price = dto.Price,
                IsActive = dto.IsActive,
                CreatedAt = System.DateTime.UtcNow
            };
            await _routeService.UpdateRouteAsync(id, route);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoute(string id)
        {
            await _routeService.DeleteRouteAsync(id);
            return Ok();
        }
    }
} 