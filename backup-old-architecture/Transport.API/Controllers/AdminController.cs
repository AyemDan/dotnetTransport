using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Transport.Application.Services;
using Transport.Domain.Entities.MongoDB;
using Transport.API.DTOs;
using Transport.Domain.Interfaces.MongoDB;
using Microsoft.AspNetCore.Authorization;

namespace Transport.API.Controllers
{
    public class ProviderApprovalDto
    {
        public string ProviderId { get; set; }
    }

    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;
        private readonly IMongoRepository<ProviderInvite> _inviteRepo;
        public AdminController(AdminService adminService, IMongoRepository<ProviderInvite> inviteRepo)
        {
            _adminService = adminService;
            _inviteRepo = inviteRepo;
        }

        [HttpPost("approve-provider")]
        public async Task<IActionResult> ApproveProvider([FromBody] ProviderApprovalDto dto)
        {
            await _adminService.ApproveProviderAsync(dto.ProviderId);
            return Ok();
        }

        [HttpGet("ongoing-trips")]
        public async Task<IActionResult> GetOngoingTrips()
        {
            var trips = await _adminService.GetOngoingTripsAsync();
            return Ok(trips);
        }

        [HttpGet("total-bookings")]
        public async Task<IActionResult> GetTotalBookings()
        {
            var total = await _adminService.GetTotalBookingsAsync();
            return Ok(total);
        }

        [HttpPost("notify")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationDto dto)
        {
            var notification = new Notification
            {
                UserId = dto.UserId,
                Message = dto.Message,
                Type = dto.Type,
                CreatedAt = System.DateTime.UtcNow
            };
            await _adminService.SendNotificationAsync(notification);
            return Ok();
        }

        [HttpPost("create-provider-invite")]
        public async Task<IActionResult> CreateProviderInvite([FromQuery] string? email, [FromQuery] int expiryMinutes = 60)
        {
            var token = Guid.NewGuid().ToString("N");
            var invite = new ProviderInvite
            {
                Token = token,
                Email = email,
                Expiry = DateTime.UtcNow.AddMinutes(expiryMinutes),
                Used = false,
                CreatedAt = DateTime.UtcNow
            };
            await _inviteRepo.AddAsync(invite);
            var link = $"/api/auth/register-provider?inviteToken={token}";
            return Ok(new { invite.Token, invite.Expiry, Link = link });
        }
    }
} 