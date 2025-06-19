using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Transport.Shared.Entities;
using Transport.Shared.Entities.MongoDB;
using Transport.Shared.DTOs;
using Transport.Shared.Enums;
using MongoDB.Bson;
using Transport.Shared.Services;

namespace ProviderApp.Controllers;

[ApiController]
[Route("api/providers")]
public class ProviderController : ControllerBase
{
    private readonly IMongoCollection<Provider> _providers;
    private readonly IRouteService _routeService;
    private readonly ITripService _tripService;
    private readonly ILogger<ProviderController> _logger;
    private readonly HttpClient _httpClient;

    public ProviderController(
        IMongoDatabase database,
        IRouteService routeService,
        ITripService tripService,
        ILogger<ProviderController> logger,
        HttpClient httpClient
    )
    {
        _providers = database.GetCollection<Provider>("providers");
        _routeService = routeService;
        _tripService = tripService;
        _logger = logger;
        _httpClient = httpClient;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterProvider([FromBody] ProviderRegistrationDto registrationDto)
    {
        try
        {
            var existingProvider = await _providers.Find(p => p.Email == registrationDto.Email).FirstOrDefaultAsync();
            if (existingProvider != null)
            {
                return BadRequest("Provider with this email already exists");
            }

            var provider = new Provider(registrationDto.Email, registrationDto.Password)
            {
                Name = registrationDto.Name,
                CompanyName = registrationDto.CompanyName,
                LicenseNumber = registrationDto.LicenseNumber,
                ContactNumber = registrationDto.ContactNumber,
                Address = registrationDto.Address,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _providers.InsertOneAsync(provider);

            await _httpClient.PostAsJsonAsync("http://localhost:5128/api/auth/register", new
            {
                UserId = provider.Id,
                Email = provider.Email,
                Password = registrationDto.Password,
                Role = provider.Role.ToString()
            });

            return Ok(new { Message = "Provider registered successfully", ProviderId = provider.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering provider");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{providerId}")]
    public async Task<IActionResult> GetProvider(string providerId)
    {
        try
        {
            if (!Guid.TryParse(providerId, out var providerGuid))
            {
                return BadRequest("Invalid provider ID format");
            }

            var provider = await _providers.Find(p => p.Id == providerGuid).FirstOrDefaultAsync();
            if (provider == null)
            {
                return NotFound("Provider not found");
            }

            return Ok(provider);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving provider");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{providerId}")]
    public async Task<IActionResult> UpdateProvider(string providerId, [FromBody] ProviderUpdateDto updateDto)
    {
        try
        {
            if (!Guid.TryParse(providerId, out var providerGuid))
            {
                return BadRequest("Invalid provider ID format");
            }

            var update = Builders<Provider>.Update
                .Set(p => p.Name, updateDto.Name)
                .Set(p => p.ContactNumber, updateDto.ContactNumber)
                .Set(p => p.LicenseNumber, updateDto.LicenseNumber)
                .Set(p => p.UpdatedAt, DateTime.UtcNow);

            var result = await _providers.UpdateOneAsync(p => p.Id == providerGuid, update);
            if (result.MatchedCount == 0)
            {
                return NotFound("Provider not found");
            }

            return Ok("Provider updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating provider");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProviders()
    {
        try
        {
            var providers = await _providers.Find(p => p.IsActive).ToListAsync();
            return Ok(providers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving providers");
            return StatusCode(500, "Internal server error");
        }
    }

    // Route Management - Using Service Layer
    [HttpPost("{providerId}/routes")]
    public async Task<IActionResult> CreateRoute(string providerId, [FromBody] RouteDto routeDto)
    {
        try
        {
            if (!Guid.TryParse(providerId, out var providerGuid))
            {
                return BadRequest("Invalid provider ID format");
            }

            var route = new Transport.Shared.Entities.MongoDB.Route
            {
                Name = routeDto.Name,
                Stops = routeDto.Stops?.Select(s => new RouteStop
                {
                    Name = s.Name,
                    Order = s.Order,
                    PriceFromStart = s.PriceFromStart
                }).ToList() ?? new List<RouteStop>(),
                Schedule = routeDto.Schedule,
                Price = routeDto.Price,
                IsActive = routeDto.IsActive
            };

            var createdRoute = await _routeService.CreateRouteAsync(route);
            return Ok(new { Message = "Route created successfully", RouteId = createdRoute.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating route");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{providerId}/routes")]
    public async Task<IActionResult> GetProviderRoutes(string providerId)
    {
        try
        {
            if (!Guid.TryParse(providerId, out var providerGuid))
            {
                return BadRequest("Invalid provider ID format");
            }

            var routes = await _routeService.GetRoutesByProviderIdAsync(providerGuid);
            return Ok(routes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving provider routes");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("routes/{routeId}")]
    public async Task<IActionResult> UpdateRoute(string routeId, [FromBody] RouteDto routeDto)
    {
        try
        {
            if (!Guid.TryParse(routeId, out var routeGuid))
            {
                return BadRequest("Invalid route ID format");
            }

            var existingRoute = await _routeService.GetRouteAsync(routeGuid);
            if (existingRoute == null)
            {
                return NotFound("Route not found");
            }

            existingRoute.Name = routeDto.Name;
            existingRoute.Stops = routeDto.Stops?.Select(s => new RouteStop
            {
                Name = s.Name,
                Order = s.Order,
                PriceFromStart = s.PriceFromStart
            }).ToList() ?? new List<RouteStop>();
            existingRoute.Schedule = routeDto.Schedule;
            existingRoute.Price = routeDto.Price;
            existingRoute.IsActive = routeDto.IsActive;

            var success = await _routeService.UpdateRouteAsync(existingRoute);
            if (!success)
            {
                return StatusCode(500, "Failed to update route");
            }

            return Ok("Route updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating route");
            return StatusCode(500, "Internal server error");
        }
    }

    // Trip Management - Using Service Layer
    [HttpPost("routes/{routeId}/trips")]
    public async Task<IActionResult> CreateTrip(string routeId, [FromBody] TripDto tripDto)
    {
        try
        {
            if (!Guid.TryParse(routeId, out var routeGuid))
            {
                return BadRequest("Invalid route ID format");
            }

            var trip = new Transport.Shared.Entities.MongoDB.Trip
            {
                // Note: Trip entity properties need to be implemented based on actual structure
                CreatedAt = DateTime.UtcNow
            };

            var createdTrip = await _tripService.CreateTripAsync(trip);
            return Ok(new { Message = "Trip created successfully", TripId = createdTrip.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating trip");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("routes/{routeId}/trips")]
    public async Task<IActionResult> GetRouteTrips(string routeId)
    {
        try
        {
            if (!Guid.TryParse(routeId, out var routeGuid))
            {
                return BadRequest("Invalid route ID format");
            }

            var trips = await _tripService.GetTripsByRouteIdAsync(routeGuid);
            return Ok(trips);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving route trips");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("trips/{tripId}/start")]
    public async Task<IActionResult> StartTrip(string tripId)
    {
        try
        {
            if (!Guid.TryParse(tripId, out var tripGuid))
            {
                return BadRequest("Invalid trip ID format");
            }

            var success = await _tripService.StartTripAsync(tripGuid);
            if (!success)
            {
                return NotFound("Trip not found or could not be started");
            }

            return Ok("Trip started successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting trip");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("trips/{tripId}/end")]
    public async Task<IActionResult> EndTrip(string tripId)
    {
        try
        {
            if (!Guid.TryParse(tripId, out var tripGuid))
            {
                return BadRequest("Invalid trip ID format");
            }

            var success = await _tripService.EndTripAsync(tripGuid);
            if (!success)
            {
                return NotFound("Trip not found or could not be ended");
            }

            return Ok("Trip ended successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ending trip");
            return StatusCode(500, "Internal server error");
        }
    }
}

public class ProviderRegistrationDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string? ContactNumber { get; set; }
    public string? Address { get; set; }
}

public class ProviderUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string? ContactNumber { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
}

public class TripDto
{
    public Guid DriverId { get; set; }
    public Guid OrganizationId { get; set; }
    public DateTime ScheduledDeparture { get; set; }
    public DateTime ScheduledArrival { get; set; }
    public string? VehicleId { get; set; }
    public int Capacity { get; set; }
}
