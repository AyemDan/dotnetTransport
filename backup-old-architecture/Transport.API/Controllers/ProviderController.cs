using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Transport.Application.Services;
using Transport.Domain.Entities.MongoDB;
using Transport.API.DTOs;
using Transport.Domain.Interfaces;
using Transport.Domain.Interfaces.MongoDB;
using Microsoft.AspNetCore.Authorization;

namespace Transport.API.Controllers
{
    [ApiController]
    [Route("api/provider")]
    [Authorize(Roles = "OrganizationAdmin,Provider")]
    public class ProviderController : ControllerBase
    {
        private readonly ProviderService _providerService;
        private readonly IMongoRepository<ProviderDocument> _docRepo;
        public ProviderController(ProviderService providerService, IMongoRepository<ProviderDocument> docRepo)
        {
            _providerService = providerService;
            _docRepo = docRepo;
        }

        [HttpPost("update-booking-status")]
        public async Task<IActionResult> UpdateBookingStatus([FromBody] BookingStatusDto dto)
        {
            await _providerService.UpdateBookingStatusAsync(dto.BookingId, dto.Status);
            return Ok();
        }

        [HttpPost("set-carpool-seats")]
        public async Task<IActionResult> SetCarpoolSeats([FromQuery] Guid carpoolId, [FromQuery] int seats)
        {
            await _providerService.SetCarpoolSeatsAsync(carpoolId, seats);
            return Ok();
        }

        [HttpGet("earnings")]
        public async Task<IActionResult> GetEarnings([FromQuery] Guid providerId)
        {
            var earnings = await _providerService.GetEarningsAsync(providerId);
            return Ok(earnings);
        }

        [HttpPost("notify")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationDto dto)
        {
            var notification = new Notification
            {
                UserId = dto.UserId,
                Message = dto.Message,
                Type = dto.Type,
                CreatedAt = DateTime.UtcNow
            };
            await _providerService.SendNotificationAsync(notification);
            return Ok();
        }

        [HttpPost("verify-driver")]
        public async Task<IActionResult> VerifyDriver([FromQuery] string driverId)
        {
            await _providerService.VerifyDriverAsync(driverId);
            return Ok();
        }

        [HttpPost("assign-trip")]
        public async Task<IActionResult> AssignTripToDriver([FromQuery] string driverId, [FromQuery] Guid tripId)
        {
            await _providerService.AssignTripToDriverAsync(driverId, tripId);
            return Ok();
        }

        [HttpPost("upload-document")]
        public async Task<IActionResult> UploadDocument([FromBody] ProviderDocument doc)
        {
            doc.Status = "Pending";
            doc.UploadedAt = DateTime.UtcNow;
            await _docRepo.AddAsync(doc);
            return Ok();
        }

        [HttpGet("documents")]
        public async Task<IActionResult> GetProviderDocuments([FromQuery] Guid providerId)
        {
            var docs = await _docRepo.FindAsync(d => d.ProviderId == providerId);
            return Ok(docs);
        }

        [HttpGet("all-documents")]
        public async Task<IActionResult> GetAllDocuments()
        {
            var docs = await _docRepo.GetAllAsync();
            return Ok(docs);
        }

        [HttpPost("review-document")]
        public async Task<IActionResult> ReviewDocument([FromQuery] string docId, [FromQuery] string status, [FromQuery] string? note)
        {
            var doc = await _docRepo.GetByIdAsync(docId);
            if (doc == null) return NotFound();
            doc.Status = status;
            doc.AdminNote = note;
            await _docRepo.UpdateAsync(docId, doc);
            return Ok();
        }

        [HttpGet("payment-logs")]
        public async Task<IActionResult> GetPaymentLogs([FromQuery] Guid providerId, [FromServices] IMongoRepository<PaymentLog> paymentRepo)
        {
            var logs = await paymentRepo.FindAsync(l => l.UserId == providerId);
            return Ok(logs);
        }
    }
} 
 