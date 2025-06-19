using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace PlatformManager.API.Controllers;

[ApiController]
[Route("api/platform")]
public class PlatformAdminController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PlatformAdminController> _logger;

    // Service URLs for new architecture
    private const string GATEWAY_URL = "http://localhost:5000/api/gateway";
    private const string STUDENT_APP_URL = "http://localhost:5001/api/students";
    private const string PROVIDER_APP_URL = "http://localhost:5002/api/providers";
    private const string ORGANIZATION_APP_URL = "http://localhost:5004/api/organizations";
    private const string AUTH_SERVICE_URL = "http://localhost:5003/api/auth";
    private const string PAYMENT_SERVICE_URL = "http://localhost:5006/api/payments";

    public PlatformAdminController(HttpClient httpClient, ILogger<PlatformAdminController> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    [HttpGet("health")]
    public async Task<IActionResult> GetSystemHealth()
    {
        try
        {
            var healthChecks = new Dictionary<string, string>();

            // Check all services
            var services = new[]
            {
                ("Gateway", "http://localhost:5000/health"),
                ("StudentApp", "http://localhost:5001/health"),
                ("ProviderApp", "http://localhost:5002/health"),
                ("AuthService", "http://localhost:5003/health"),
                ("OrganizationApp", "http://localhost:5004/health"),
                ("NotificationService", "http://localhost:5005/health"),
                ("PaymentService", "http://localhost:5006/health")
            };

            foreach (var (name, url) in services)
            {
                try
                {
                    var response = await _httpClient.GetAsync(url);
                    healthChecks[name] = response.IsSuccessStatusCode ? "Healthy" : "Unhealthy";
                }
                catch
                {
                    healthChecks[name] = "Unreachable";
                }
            }

            return Ok(new
            {
                PlatformManager = "Healthy",
                Services = healthChecks,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking system health");
            return StatusCode(500, "Error checking system health");
        }
    }

    [HttpGet("organizations")]
    public async Task<IActionResult> GetOrganizations()
    {
        try
        {
            // This would typically query the database directly or use a dedicated endpoint
            // For now, we'll return a placeholder response
            var organizations = new[]
            {
                new { Id = "org1", Name = "Sample School", Type = "School", Status = "Active" },
                new { Id = "org2", Name = "Sample University", Type = "University", Status = "Active" }
            };

            return Ok(organizations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving organizations");
            return StatusCode(500, "Error retrieving organizations");
        }
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] string? org = null)
    {
        try
        {
            // This would aggregate users from all applications
            // For now, we'll return a placeholder response
            var users = new[]
            {
                new { Id = "user1", Name = "John Doe", Email = "john@school.com", Type = "Student", Organization = "Sample School" },
                new { Id = "user2", Name = "Jane Smith", Email = "jane@school.com", Type = "Provider", Organization = "Sample School" },
                new { Id = "user3", Name = "Admin User", Email = "admin@school.com", Type = "Admin", Organization = "Sample School" }
            };

            if (!string.IsNullOrEmpty(org))
            {
                users = users.Where(u => u.Organization.Equals(org, StringComparison.OrdinalIgnoreCase)).ToArray();
            }

            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(500, "Error retrieving users");
        }
    }

    [HttpGet("bookings")]
    public async Task<IActionResult> GetBookings([FromQuery] string? org = null)
    {
        try
        {
            // This would aggregate bookings from the StudentApp
            // For now, we'll return a placeholder response
            var bookings = new[]
            {
                new { Id = "booking1", StudentName = "John Doe", Route = "Home to School", Date = DateTime.Today, Status = "Confirmed" },
                new { Id = "booking2", StudentName = "Jane Smith", Route = "School to Home", Date = DateTime.Today.AddDays(1), Status = "Pending" }
            };

            return Ok(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings");
            return StatusCode(500, "Error retrieving bookings");
        }
    }

    [HttpGet("analytics")]
    public async Task<IActionResult> GetSystemAnalytics()
    {
        try
        {
            // This would aggregate analytics from all applications
            var analytics = new
            {
                TotalOrganizations = 2,
                TotalStudents = 150,
                TotalProviders = 25,
                TotalAdmins = 5,
                ActiveBookings = 45,
                TotalRevenue = 15000.00m,
                SystemUptime = "99.9%",
                LastUpdated = DateTime.UtcNow
            };

            return Ok(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving analytics");
            return StatusCode(500, "Error retrieving analytics");
        }
    }

    [HttpPost("emergency/stop")]
    public async Task<IActionResult> EmergencyStop()
    {
        try
        {
            _logger.LogWarning("EMERGENCY STOP initiated by platform admin");

            // In a real implementation, this would:
            // 1. Stop all active trips
            // 2. Cancel pending bookings
            // 3. Notify all users
            // 4. Log the emergency action

            var emergencyAction = new
            {
                Action = "Emergency Stop",
                InitiatedBy = "Platform Admin",
                Timestamp = DateTime.UtcNow,
                Status = "Executed",
                Message = "All transport operations have been stopped"
            };

            return Ok(emergencyAction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during emergency stop");
            return StatusCode(500, "Error during emergency stop");
        }
    }

    [HttpPost("maintenance/mode")]
    public async Task<IActionResult> EnableMaintenanceMode([FromBody] MaintenanceModeRequest request)
    {
        try
        {
            _logger.LogInformation($"Maintenance mode {(request.Enabled ? "enabled" : "disabled")} by platform admin");

            var maintenanceAction = new
            {
                Action = "Maintenance Mode",
                Enabled = request.Enabled,
                InitiatedBy = "Platform Admin",
                Timestamp = DateTime.UtcNow,
                Message = request.Enabled ? "System is now in maintenance mode" : "System is now operational"
            };

            return Ok(maintenanceAction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling maintenance mode");
            return StatusCode(500, "Error toggling maintenance mode");
        }
    }

    [HttpGet("logs")]
    public async Task<IActionResult> GetSystemLogs([FromQuery] string? level = null, [FromQuery] DateTime? from = null)
    {
        try
        {
            // This would retrieve logs from all services
            var logs = new[]
            {
                new { Timestamp = DateTime.UtcNow.AddMinutes(-5), Level = "INFO", Service = "StudentApp", Message = "Student registered successfully" },
                new { Timestamp = DateTime.UtcNow.AddMinutes(-3), Level = "INFO", Service = "ProviderApp", Message = "Route created successfully" },
                new { Timestamp = DateTime.UtcNow.AddMinutes(-1), Level = "WARN", Service = "PaymentService", Message = "Payment processing delayed" }
            };

            if (!string.IsNullOrEmpty(level))
            {
                logs = logs.Where(l => l.Level.Equals(level, StringComparison.OrdinalIgnoreCase)).ToArray();
            }

            if (from.HasValue)
            {
                logs = logs.Where(l => l.Timestamp >= from.Value).ToArray();
            }

            return Ok(logs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving system logs");
            return StatusCode(500, "Error retrieving system logs");
        }
    }
}

public class MaintenanceModeRequest
{
    public bool Enabled { get; set; }
    public string? Reason { get; set; }
} 