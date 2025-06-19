using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Transport.Shared.Entities;
using Transport.Shared.Enums;
using MongoDB.Bson;
using Transport.Shared.Services;

namespace OrganizationApp.Controllers;

[ApiController]
[Route("api/organizations")]
public class OrganizationController : ControllerBase
{
    private readonly IMongoCollection<Organization> _organizations;
    private readonly IMongoCollection<Admin> _admins;
    private readonly ISubscriptionService _subscriptionService;
    private readonly ILogger<OrganizationController> _logger;
    private readonly HttpClient _httpClient;

    public OrganizationController(
        IMongoDatabase database,
        ISubscriptionService subscriptionService,
        ILogger<OrganizationController> logger,
        HttpClient httpClient
    )
    {
        _organizations = database.GetCollection<Organization>("organizations");
        _admins = database.GetCollection<Admin>("admins");
        _subscriptionService = subscriptionService;
        _logger = logger;
        _httpClient = httpClient;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterOrganization([FromBody] OrganizationRegistrationDto registrationDto)
    {
        try
        {
            var organization = new Organization
            {
                Id = Guid.NewGuid(),
                Name = registrationDto.Name,
                Address = registrationDto.Address,
                CreatedAt = DateTime.UtcNow
            };

            await _organizations.InsertOneAsync(organization);

            var admin = new Admin(registrationDto.AdminEmail, registrationDto.AdminPassword, AdminLevel.SuperAdmin)
            {
                Name = registrationDto.AdminName,
                CreatedAt = DateTime.UtcNow
            };

            await _admins.InsertOneAsync(admin);

            await _httpClient.PostAsJsonAsync("http://localhost:5128/api/auth/register", new
            {
                UserId = admin.Id,
                Email = admin.Email,
                Password = registrationDto.AdminPassword,
                Role = admin.Role.ToString()
            });

            return Ok(new { Message = "Organization registered successfully", OrganizationId = organization.Id, AdminId = admin.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering organization");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{organizationId}")]
    public async Task<IActionResult> GetOrganization(string organizationId)
    {
        try
        {
            if (!Guid.TryParse(organizationId, out var orgGuid))
            {
                return BadRequest("Invalid organization ID format");
            }

            var organization = await _organizations.Find(o => o.Id == orgGuid).FirstOrDefaultAsync();
            if (organization == null)
            {
                return NotFound("Organization not found");
            }

            return Ok(organization);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving organization");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOrganizations()
    {
        try
        {
            var organizations = await _organizations.Find(_ => true).ToListAsync();
            return Ok(organizations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving organizations");
            return StatusCode(500, "Internal server error");
        }
    }

    // Subscription Management - Using Service Layer
    [HttpPost("{organizationId}/subscriptions")]
    public async Task<IActionResult> CreateSubscription(string organizationId, [FromBody] SubscriptionDto subscriptionDto)
    {
        try
        {
            if (!Guid.TryParse(organizationId, out var orgGuid))
            {
                return BadRequest("Invalid organization ID format");
            }

            var subscription = new Transport.Shared.Entities.MongoDB.Subscription
            {
                UserId = subscriptionDto.UserId,
                RouteId = Guid.Empty, // Note: This would need to be set based on business logic
                Period = subscriptionDto.PlanType,
                StartDate = subscriptionDto.StartDate,
                EndDate = subscriptionDto.EndDate,
                Amount = subscriptionDto.Amount,
                PaymentStatus = subscriptionDto.PaymentMethod
            };

            var createdSubscription = await _subscriptionService.CreateSubscriptionAsync(subscription);
            return Ok(new { Message = "Subscription created successfully", SubscriptionId = createdSubscription.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating subscription");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{organizationId}/subscriptions")]
    public async Task<IActionResult> GetOrganizationSubscriptions(string organizationId)
    {
        try
        {
            if (!Guid.TryParse(organizationId, out var orgGuid))
            {
                return BadRequest("Invalid organization ID format");
            }

            // Note: This would require adding a method to get subscriptions by organization ID
            // For now, we'll return a placeholder response
            return Ok(new { Message = "Organization subscriptions endpoint - implementation needed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving organization subscriptions");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("subscriptions/{subscriptionId}/cancel")]
    public async Task<IActionResult> CancelSubscription(string subscriptionId)
    {
        try
        {
            if (!Guid.TryParse(subscriptionId, out var subscriptionGuid))
            {
                return BadRequest("Invalid subscription ID format");
            }

            var success = await _subscriptionService.CancelSubscriptionAsync(subscriptionGuid);
            if (!success)
            {
                return NotFound("Subscription not found or could not be cancelled");
            }

            return Ok("Subscription cancelled successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling subscription");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("subscriptions/{subscriptionId}/activate")]
    public async Task<IActionResult> ActivateSubscription(string subscriptionId)
    {
        try
        {
            if (!Guid.TryParse(subscriptionId, out var subscriptionGuid))
            {
                return BadRequest("Invalid subscription ID format");
            }

            var success = await _subscriptionService.ActivateSubscriptionAsync(subscriptionGuid);
            if (!success)
            {
                return NotFound("Subscription not found or could not be activated");
            }

            return Ok("Subscription activated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating subscription");
            return StatusCode(500, "Internal server error");
        }
    }

    // Admin Management
    [HttpPost("{organizationId}/admins")]
    public async Task<IActionResult> AddAdmin(string organizationId, [FromBody] AdminRegistrationDto adminDto)
    {
        try
        {
            if (!Guid.TryParse(organizationId, out var orgGuid))
            {
                return BadRequest("Invalid organization ID format");
            }

            var admin = new Admin(adminDto.Email, adminDto.Password, adminDto.AdminLevel)
            {
                Name = adminDto.Name,
                OrganizationId = orgGuid,
                CreatedAt = DateTime.UtcNow
            };

            await _admins.InsertOneAsync(admin);

            await _httpClient.PostAsJsonAsync("http://localhost:5128/api/auth/register", new
            {
                UserId = admin.Id,
                Email = admin.Email,
                Password = adminDto.Password,
                Role = admin.Role.ToString()
            });

            return Ok(new { Message = "Admin added successfully", AdminId = admin.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding admin");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{organizationId}/admins")]
    public async Task<IActionResult> GetOrganizationAdmins(string organizationId)
    {
        try
        {
            if (!Guid.TryParse(organizationId, out var orgGuid))
            {
                return BadRequest("Invalid organization ID format");
            }

            var admins = await _admins.Find(a => a.OrganizationId == orgGuid).ToListAsync();
            return Ok(admins);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving organization admins");
            return StatusCode(500, "Internal server error");
        }
    }
}

public class OrganizationRegistrationDto
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string AdminName { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
    public string AdminPassword { get; set; } = string.Empty;
}

public class AdminRegistrationDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public AdminLevel AdminLevel { get; set; } = AdminLevel.Support;
}

public class SubscriptionDto
{
    public Guid UserId { get; set; }
    public string PlanType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
}
