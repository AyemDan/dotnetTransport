using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace API.Gateway.Controllers;

[ApiController]
[Route("api/gateway")]
[Authorize]
public class GatewayController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly ServiceSettings _serviceSettings;
    private readonly ILogger<GatewayController> _logger;

    // Service URLs
    private const string STUDENT_APP_URL = "http://localhost:5001";
    private const string PROVIDER_APP_URL = "http://localhost:5002";
    private const string AUTH_SERVICE_URL = "http://localhost:5003";
    private const string ORGANIZATION_APP_URL = "http://localhost:5004";
    private const string NOTIFICATION_SERVICE_URL = "http://localhost:5005";
    private const string PAYMENT_SERVICE_URL = "http://localhost:5006";

    public GatewayController(
        HttpClient httpClient,
        IOptions<ServiceSettings> serviceSettings,
        ILogger<GatewayController> logger
    )
    {
        _httpClient = httpClient;
        _serviceSettings = serviceSettings.Value;
        _logger = logger;
    }

    #region Auth Service Endpoints

    [HttpPost("auth/register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] object request)
    {
        return await ForwardRequest(
            $"{_serviceSettings.AuthService}/api/auth/register",
            HttpMethod.Post,
            request
        );
    }

    [HttpPost("auth/login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] object request)
    {
        return await ForwardRequest(
            $"{_serviceSettings.AuthService}/api/auth/login",
            HttpMethod.Post,
            request
        );
    }

    [HttpPost("auth/register-org-admin")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> RegisterOrgAdmin([FromBody] object request)
    {
        return await ForwardRequest(
            $"{_serviceSettings.AuthService}/api/auth/register-org-admin",
            HttpMethod.Post,
            request
        );
    }

    #endregion

    #region Provider Service Endpoints

    [HttpGet("providers")]
    [Authorize(Policy = "OrgAdminOrProvider")]
    public async Task<IActionResult> GetProviders()
    {
        return await ForwardRequest(
            $"{_serviceSettings.ProviderService}/api/provider",
            HttpMethod.Get
        );
    }

    [HttpGet("providers/{id}")]
    [Authorize(Policy = "OrgAdminOrProvider")]
    public async Task<IActionResult> GetProviderById(Guid id)
    {
        return await ForwardRequest(
            $"{_serviceSettings.ProviderService}/api/provider/{id}",
            HttpMethod.Get
        );
    }

    [HttpPost("providers")]
    [Authorize(Policy = "OrgAdminOnly")]
    public async Task<IActionResult> CreateProvider([FromBody] object provider)
    {
        return await ForwardRequest(
            $"{_serviceSettings.ProviderService}/api/provider",
            HttpMethod.Post,
            provider
        );
    }

    [HttpPut("providers/{id}")]
    [Authorize(Policy = "OrgAdminOrProvider")]
    public async Task<IActionResult> UpdateProvider(Guid id, [FromBody] object provider)
    {
        return await ForwardRequest(
            $"{_serviceSettings.ProviderService}/api/provider/{id}",
            HttpMethod.Put,
            provider
        );
    }

    [HttpDelete("providers/{id}")]
    [Authorize(Policy = "OrgAdminOnly")]
    public async Task<IActionResult> DeleteProvider(Guid id)
    {
        return await ForwardRequest(
            $"{_serviceSettings.ProviderService}/api/provider/{id}",
            HttpMethod.Delete
        );
    }

    [HttpPost("providers/update-booking-status")]
    [Authorize(Policy = "ProviderOnly")]
    public async Task<IActionResult> UpdateBookingStatus([FromBody] object statusDto)
    {
        return await ForwardRequest(
            $"{_serviceSettings.ProviderService}/api/provider/update-booking-status",
            HttpMethod.Post,
            statusDto
        );
    }

    [HttpGet("providers/earnings/{providerId}")]
    [Authorize(Policy = "ProviderOnly")]
    public async Task<IActionResult> GetProviderEarnings(Guid providerId)
    {
        return await ForwardRequest(
            $"{_serviceSettings.ProviderService}/api/provider/earnings/{providerId}",
            HttpMethod.Get
        );
    }

    // Route management endpoints
    [HttpGet("providers/routes")]
    public async Task<IActionResult> GetRoutes()
    {
        return await ForwardRequest(
            $"{_serviceSettings.ProviderService}/api/provider/routes",
            HttpMethod.Get
        );
    }

    [HttpPost("providers/routes")]
    public async Task<IActionResult> CreateRoute([FromBody] object route)
    {
        return await ForwardRequest(
            $"{_serviceSettings.ProviderService}/api/provider/routes",
            HttpMethod.Post,
            route
        );
    }

    [HttpPut("providers/routes/{id}")]
    public async Task<IActionResult> UpdateRoute(Guid id, [FromBody] object route)
    {
        return await ForwardRequest(
            $"{_serviceSettings.ProviderService}/api/provider/routes/{id}",
            HttpMethod.Put,
            route
        );
    }

    [HttpDelete("providers/routes/{id}")]
    public async Task<IActionResult> DeleteRoute(Guid id)
    {
        return await ForwardRequest(
            $"{_serviceSettings.ProviderService}/api/provider/routes/{id}",
            HttpMethod.Delete
        );
    }

    #endregion

    #region Organization Service Endpoints

    [HttpGet("organizations")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetOrganizations()
    {
        return await ForwardRequest(
            $"{_serviceSettings.OrganizationService}/api/organization",
            HttpMethod.Get
        );
    }

    [HttpGet("organizations/{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetOrganizationById(Guid id)
    {
        return await ForwardRequest(
            $"{_serviceSettings.OrganizationService}/api/organization/{id}",
            HttpMethod.Get
        );
    }

    [HttpPost("organizations")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> CreateOrganization([FromBody] object organization)
    {
        return await ForwardRequest(
            $"{_serviceSettings.OrganizationService}/api/organization",
            HttpMethod.Post,
            organization
        );
    }

    [HttpPut("organizations/{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateOrganization(Guid id, [FromBody] object organization)
    {
        return await ForwardRequest(
            $"{_serviceSettings.OrganizationService}/api/organization/{id}",
            HttpMethod.Put,
            organization
        );
    }

    [HttpDelete("organizations/{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteOrganization(Guid id)
    {
        return await ForwardRequest(
            $"{_serviceSettings.OrganizationService}/api/organization/{id}",
            HttpMethod.Delete
        );
    }

    [HttpGet("organizations/{orgId}/users")]
    [Authorize(Policy = "AdminOrOrgAdmin")]
    public async Task<IActionResult> GetUsersByOrganization(Guid orgId)
    {
        return await ForwardRequest(
            $"{_serviceSettings.OrganizationService}/api/organization/{orgId}/users",
            HttpMethod.Get
        );
    }

    [HttpPost("organizations/{orgId}/admins")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> CreateOrganizationAdmin(Guid orgId, [FromBody] object admin)
    {
        return await ForwardRequest(
            $"{_serviceSettings.OrganizationService}/api/organization/{orgId}/admins",
            HttpMethod.Post,
            admin
        );
    }

    #endregion

    #region Student Service Endpoints

    [HttpGet("students")]
    [Authorize(Policy = "OrgAdminOrStudent")]
    public async Task<IActionResult> GetStudents()
    {
        return await ForwardRequest(
            $"{_serviceSettings.StudentService}/api/student",
            HttpMethod.Get
        );
    }

    [HttpGet("students/{id}")]
    [Authorize(Policy = "OrgAdminOrStudent")]
    public async Task<IActionResult> GetStudentById(Guid id)
    {
        return await ForwardRequest(
            $"{_serviceSettings.StudentService}/api/student/{id}",
            HttpMethod.Get
        );
    }

    [HttpPost("students")]
    [Authorize(Policy = "OrgAdminOnly")]
    public async Task<IActionResult> CreateStudent([FromBody] object student)
    {
        return await ForwardRequest(
            $"{_serviceSettings.StudentService}/api/student",
            HttpMethod.Post,
            student
        );
    }

    [HttpPut("students/{id}")]
    [Authorize(Policy = "OrgAdminOrStudent")]
    public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] object student)
    {
        return await ForwardRequest(
            $"{_serviceSettings.StudentService}/api/student/{id}",
            HttpMethod.Put,
            student
        );
    }

    [HttpDelete("students/{id}")]
    [Authorize(Policy = "OrgAdminOnly")]
    public async Task<IActionResult> DeleteStudent(Guid id)
    {
        return await ForwardRequest(
            $"{_serviceSettings.StudentService}/api/student/{id}",
            HttpMethod.Delete
        );
    }

    [HttpPost("students/book-trip")]
    [Authorize(Policy = "StudentOnly")]
    public async Task<IActionResult> BookTrip([FromBody] object bookingDto)
    {
        return await ForwardRequest(
            $"{_serviceSettings.StudentService}/api/student/book-trip",
            HttpMethod.Post,
            bookingDto
        );
    }

    [HttpGet("students/{studentId}/bookings")]
    [Authorize(Policy = "OrgAdminOrStudent")]
    public async Task<IActionResult> GetStudentBookings(Guid studentId)
    {
        return await ForwardRequest(
            $"{_serviceSettings.StudentService}/api/student/{studentId}/bookings",
            HttpMethod.Get
        );
    }

    [HttpPost("students/subscribe")]
    [Authorize(Policy = "StudentOnly")]
    public async Task<IActionResult> SubscribeToRoute([FromBody] object subscriptionDto)
    {
        return await ForwardRequest(
            $"{_serviceSettings.StudentService}/api/student/subscribe",
            HttpMethod.Post,
            subscriptionDto
        );
    }

    [HttpGet("students/{studentId}/subscriptions")]
    [Authorize(Policy = "OrgAdminOrStudent")]
    public async Task<IActionResult> GetStudentSubscriptions(Guid studentId)
    {
        return await ForwardRequest(
            $"{_serviceSettings.StudentService}/api/student/{studentId}/subscriptions",
            HttpMethod.Get
        );
    }

    // Platform Manager endpoints
    [HttpGet("students/payment-logs")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetPaymentLogs()
    {
        return await ForwardRequest(
            $"{_serviceSettings.StudentService}/api/student/payment-logs",
            HttpMethod.Get
        );
    }

    [HttpPost("students/reverse-payment/{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> ReversePayment(string id, [FromBody] object reversalData)
    {
        return await ForwardRequest(
            $"{_serviceSettings.StudentService}/api/student/reverse-payment/{id}",
            HttpMethod.Post,
            reversalData
        );
    }

    [HttpGet("students/payment-analytics/{orgId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetPaymentAnalytics(Guid orgId)
    {
        return await ForwardRequest(
            $"{_serviceSettings.StudentService}/api/student/payment-analytics/{orgId}",
            HttpMethod.Get
        );
    }

    [HttpGet("students/booking-analytics/{orgId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetBookingAnalytics(Guid orgId)
    {
        return await ForwardRequest(
            $"{_serviceSettings.StudentService}/api/student/booking-analytics/{orgId}",
            HttpMethod.Get
        );
    }

    [HttpPost("students/cancel-booking/{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> CancelBooking(string id, [FromBody] object cancelData)
    {
        return await ForwardRequest(
            $"{_serviceSettings.StudentService}/api/student/cancel-booking/{id}",
            HttpMethod.Post,
            cancelData
        );
    }

    #endregion

    #region RFID Card Service Endpoints

    [HttpGet("rfidcards")]
    public async Task<IActionResult> GetAllCards()
    {
        return await ForwardRequest(
            $"{_serviceSettings.RFIDCardService}/api/rfidcard",
            HttpMethod.Get
        );
    }

    [HttpGet("rfidcards/{cardNumber}")]
    public async Task<IActionResult> GetCardByNumber(string cardNumber)
    {
        return await ForwardRequest(
            $"{_serviceSettings.RFIDCardService}/api/rfidcard/{cardNumber}",
            HttpMethod.Get
        );
    }

    [HttpPost("rfidcards")]
    public async Task<IActionResult> AddCard([FromBody] object card)
    {
        return await ForwardRequest(
            $"{_serviceSettings.RFIDCardService}/api/rfidcard",
            HttpMethod.Post,
            card
        );
    }

    [HttpPost("rfidcards/topup")]
    public async Task<IActionResult> TopUp([FromBody] object topUpDto)
    {
        return await ForwardRequest(
            $"{_serviceSettings.RFIDCardService}/api/rfidcard/topup",
            HttpMethod.Post,
            topUpDto
        );
    }

    [HttpPost("rfidcards/tapin")]
    public async Task<IActionResult> TapIn([FromBody] object tapInDto)
    {
        return await ForwardRequest(
            $"{_serviceSettings.RFIDCardService}/api/rfidcard/tapin",
            HttpMethod.Post,
            tapInDto
        );
    }

    [HttpPost("rfidcards/tapout")]
    public async Task<IActionResult> TapOut([FromBody] object tapOutDto)
    {
        return await ForwardRequest(
            $"{_serviceSettings.RFIDCardService}/api/rfidcard/tapout",
            HttpMethod.Post,
            tapOutDto
        );
    }

    [HttpPost("rfidcards/vet-tapout")]
    public async Task<IActionResult> VetTapOut([FromBody] object vetTapOutDto)
    {
        return await ForwardRequest(
            $"{_serviceSettings.RFIDCardService}/api/rfidcard/vet-tapout",
            HttpMethod.Post,
            vetTapOutDto
        );
    }

    [HttpGet("rfidcards/balance/{cardNumber}")]
    public async Task<IActionResult> GetBalance(string cardNumber)
    {
        return await ForwardRequest(
            $"{_serviceSettings.RFIDCardService}/api/rfidcard/balance/{cardNumber}",
            HttpMethod.Get
        );
    }

    #endregion

    #region Subscription Service Endpoints

    [HttpGet("subscriptions")]
    public async Task<IActionResult> GetAllSubscriptions()
    {
        return await ForwardRequest(
            $"{_serviceSettings.SubscriptionService}/api/subscription",
            HttpMethod.Get
        );
    }

    [HttpGet("subscriptions/{id}")]
    public async Task<IActionResult> GetSubscriptionById(string id)
    {
        return await ForwardRequest(
            $"{_serviceSettings.SubscriptionService}/api/subscription/{id}",
            HttpMethod.Get
        );
    }

    [HttpPost("subscriptions")]
    public async Task<IActionResult> AddSubscription([FromBody] object subscription)
    {
        return await ForwardRequest(
            $"{_serviceSettings.SubscriptionService}/api/subscription",
            HttpMethod.Post,
            subscription
        );
    }

    [HttpPut("subscriptions/{id}")]
    public async Task<IActionResult> UpdateSubscription(string id, [FromBody] object subscription)
    {
        return await ForwardRequest(
            $"{_serviceSettings.SubscriptionService}/api/subscription/{id}",
            HttpMethod.Put,
            subscription
        );
    }

    [HttpDelete("subscriptions/{id}")]
    public async Task<IActionResult> DeleteSubscription(string id)
    {
        return await ForwardRequest(
            $"{_serviceSettings.SubscriptionService}/api/subscription/{id}",
            HttpMethod.Delete
        );
    }

    [HttpGet("subscriptions/user/{userId}")]
    public async Task<IActionResult> GetSubscriptionsByUser(Guid userId)
    {
        return await ForwardRequest(
            $"{_serviceSettings.SubscriptionService}/api/subscription/user/{userId}",
            HttpMethod.Get
        );
    }

    [HttpGet("subscriptions/route/{routeId}")]
    public async Task<IActionResult> GetSubscriptionsByRoute(Guid routeId)
    {
        return await ForwardRequest(
            $"{_serviceSettings.SubscriptionService}/api/subscription/route/{routeId}",
            HttpMethod.Get
        );
    }

    [HttpGet("subscriptions/active")]
    public async Task<IActionResult> GetActiveSubscriptions()
    {
        return await ForwardRequest(
            $"{_serviceSettings.SubscriptionService}/api/subscription/active",
            HttpMethod.Get
        );
    }

    [HttpPost("subscriptions/{id}/renew")]
    public async Task<IActionResult> RenewSubscription(string id, [FromBody] object renewDto)
    {
        return await ForwardRequest(
            $"{_serviceSettings.SubscriptionService}/api/subscription/{id}/renew",
            HttpMethod.Post,
            renewDto
        );
    }

    [HttpPost("subscriptions/{id}/cancel")]
    public async Task<IActionResult> CancelSubscription(string id)
    {
        return await ForwardRequest(
            $"{_serviceSettings.SubscriptionService}/api/subscription/{id}/cancel",
            HttpMethod.Post
        );
    }

    [HttpGet("subscriptions/analytics/{orgId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetSubscriptionAnalytics(Guid orgId)
    {
        return await ForwardRequest(
            $"{_serviceSettings.SubscriptionService}/api/subscription/analytics/{orgId}",
            HttpMethod.Get
        );
    }

    #endregion

    #region Admin Service Endpoints

    [HttpPost("admin/approve-provider")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> ApproveProvider([FromBody] object approvalDto)
    {
        return await ForwardRequest(
            $"{_serviceSettings.AdminService}/api/admin/approve-provider",
            HttpMethod.Post,
            approvalDto
        );
    }

    [HttpGet("admin/ongoing-trips")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetOngoingTrips()
    {
        return await ForwardRequest(
            $"{_serviceSettings.AdminService}/api/admin/ongoing-trips",
            HttpMethod.Get
        );
    }

    [HttpGet("admin/total-bookings")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetTotalBookings()
    {
        return await ForwardRequest(
            $"{_serviceSettings.AdminService}/api/admin/total-bookings",
            HttpMethod.Get
        );
    }

    [HttpPost("admin/notify")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> SendNotification([FromBody] object notificationDto)
    {
        return await ForwardRequest(
            $"{_serviceSettings.AdminService}/api/admin/notify",
            HttpMethod.Post,
            notificationDto
        );
    }

    [HttpGet("admin/system-stats")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetSystemStats()
    {
        return await ForwardRequest(
            $"{_serviceSettings.AdminService}/api/admin/system-stats",
            HttpMethod.Get
        );
    }

    [HttpGet("admin/provider-invites")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetProviderInvites()
    {
        return await ForwardRequest(
            $"{_serviceSettings.AdminService}/api/admin/provider-invites",
            HttpMethod.Get
        );
    }

    [HttpPost("admin/provider-invites")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> CreateProviderInvite([FromBody] object inviteDto)
    {
        return await ForwardRequest(
            $"{_serviceSettings.AdminService}/api/admin/provider-invites",
            HttpMethod.Post,
            inviteDto
        );
    }

    #endregion

    #region Cross-Service Operations

    [HttpGet("dashboard/{orgId}")]
    [Authorize(Policy = "OrgAdminOnly")]
    public async Task<IActionResult> GetOrganizationDashboard(Guid orgId)
    {
        try
        {
            // Aggregate data from multiple services
            var tasks = new[]
            {
                GetServiceData($"{_serviceSettings.OrganizationService}/api/organization/{orgId}"),
                GetServiceData($"{_serviceSettings.ProviderService}/api/provider"),
                GetServiceData($"{_serviceSettings.StudentService}/api/student"),
            };

            await Task.WhenAll(tasks);

            var dashboard = new
            {
                Organization = tasks[0].Result,
                Providers = tasks[1].Result,
                Students = tasks[2].Result,
                Timestamp = DateTime.UtcNow,
            };

            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error aggregating dashboard data: {ex.Message}");
        }
    }

    #endregion

    #region Health and Utility Endpoints

    [HttpGet("health")]
    [AllowAnonymous]
    public async Task<IActionResult> HealthCheck()
    {
        var healthStatus = new
        {
            Gateway = "Healthy",
            Services = new
            {
                StudentApp = await CheckServiceHealth(STUDENT_APP_URL),
                ProviderApp = await CheckServiceHealth(PROVIDER_APP_URL),
                OrganizationApp = await CheckServiceHealth(ORGANIZATION_APP_URL),
                AuthService = await CheckServiceHealth(AUTH_SERVICE_URL),
                NotificationService = await CheckServiceHealth(NOTIFICATION_SERVICE_URL),
                PaymentService = await CheckServiceHealth(PAYMENT_SERVICE_URL),
            },
            Timestamp = DateTime.UtcNow,
        };

        return Ok(healthStatus);
    }

    #endregion

    #region Helper Methods

    private async Task<IActionResult> ForwardRequest(
        string url,
        HttpMethod method,
        object? body = null
    )
    {
        try
        {
            var request = new HttpRequestMessage(method, url);

            // Forward headers (especially Authorization)
            foreach (var header in Request.Headers)
            {
                if (
                    header.Key.StartsWith("X-")
                    || header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase)
                )
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            // Forward query parameters
            if (Request.QueryString.HasValue)
            {
                var uriBuilder = new UriBuilder(url);
                uriBuilder.Query = Request.QueryString.Value;
                request.RequestUri = uriBuilder.Uri;
            }

            // Add body for POST/PUT requests
            if (body != null && (method == HttpMethod.Post || method == HttpMethod.Put))
            {
                var json = JsonSerializer.Serialize(body);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogInformation(
                $"Forwarded {method} request to {url} - Status: {response.StatusCode}"
            );

            if (response.IsSuccessStatusCode)
            {
                return Ok(JsonSerializer.Deserialize<object>(content));
            }

            return StatusCode((int)response.StatusCode, content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error forwarding request to {url}");
            return StatusCode(
                500,
                new { Error = "Service temporarily unavailable", Service = url }
            );
        }
    }

    private async Task<object?> GetServiceData(string url)
    {
        try
        {
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<object>(content);
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    private async Task<string> CheckServiceHealth(string serviceUrl)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{serviceUrl}/health");
            return response.IsSuccessStatusCode ? "Healthy" : "Unhealthy";
        }
        catch
        {
            return "Unreachable";
        }
    }

    #endregion
}
