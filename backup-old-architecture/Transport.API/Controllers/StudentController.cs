using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Transport.Application.Services;
using Transport.Domain.Entities.MongoDB;
using Transport.API.DTOs;
using Transport.API.Mappings;
using System.Linq;
using Transport.Domain.Interfaces.MongoDB;
using Microsoft.AspNetCore.Authorization;

namespace Transport.API.Controllers
{
    public class LocationUpdateDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public System.DateTime Timestamp { get; set; }
    }

    [ApiController]
    [Route("api/student")]
    [Authorize(Roles = "OrganizationAdmin,Student")]
    public class StudentController : ControllerBase
    {
        private readonly StudentService _studentService;
        public StudentController(StudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet("suggest-trips")]
        public async Task<IActionResult> SuggestTrips([FromQuery] Guid studentId, [FromQuery] string boardingStop, [FromQuery] string dropOffStop)
        {
            var suggestions = await _studentService.SuggestTripsAsync(studentId, boardingStop, dropOffStop);
            var result = suggestions.Select(s => new {
                Trip = s.trip,
                BoardingOrder = s.boardingOrder,
                DropOffOrder = s.dropOffOrder,
                Price = s.price
            });
            return Ok(result);
        }

        [HttpPost("book-trip")]
        public async Task<IActionResult> BookTrip([FromBody] BookingDto bookingDto)
        {
            var booking = BookingMapper.ToDomain(bookingDto);
            if (booking.SegmentPrice <= 0)
            {
                return BadRequest("Segment price must be set and greater than zero.");
            }
            await _studentService.BookTripAsync(booking);
            return Ok();
        }

        [HttpPost("join-carpool")]
        public async Task<IActionResult> JoinCarpool([FromQuery] Guid carpoolId, [FromQuery] Guid studentId)
        {
            await _studentService.JoinCarpoolAsync(carpoolId, studentId);
            return Ok();
        }

        [HttpPost("simulate-payment")]
        public async Task<IActionResult> SimulatePayment([FromBody] PaymentInfoDto paymentDto)
        {
            var payment = PaymentLogMapper.ToDomain(paymentDto);
            await _studentService.SimulatePaymentAsync(payment);
            return Ok();
        }

        [HttpPost("notify")]
        public async Task<IActionResult> SendNotification([FromBody] Notification notification)
        {
            await _studentService.SendNotificationAsync(notification);
            return Ok();
        }

        [HttpPost("trip/{tripId}/location")]
        public async Task<IActionResult> AddLocationUpdate(string tripId, [FromBody] LocationUpdateDto dto)
        {
            var update = new LocationUpdate
            {
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Timestamp = dto.Timestamp
            };
            var tripService = HttpContext.RequestServices.GetService(typeof(Transport.Application.Services.TripService)) as Transport.Application.Services.TripService;
            if (tripService != null)
                await tripService.AddLocationUpdateAsync(tripId, update);
            return Ok();
        }

        [HttpGet("trip/{tripId}/locations")]
        public async Task<IActionResult> GetLocationUpdates(string tripId)
        {
            var tripService = HttpContext.RequestServices.GetService(typeof(Transport.Application.Services.TripService)) as Transport.Application.Services.TripService;
            if (tripService != null)
            {
                var locations = await tripService.GetLocationUpdatesAsync(tripId);
                return Ok(locations);
            }
            return NotFound();
        }

        [HttpPost("save-payment-info")]
        public async Task<IActionResult> SavePaymentInfo([FromQuery] Guid userId, [FromBody] UserPreferences prefs, [FromServices] IMongoRepository<UserPreferences> prefsRepo)
        {
            var existing = (await prefsRepo.FindAsync(p => p.UserId == userId)).FirstOrDefault();
            if (existing != null)
            {
                existing.PreferredPaymentMethod = prefs.PreferredPaymentMethod;
                existing.PaymentDetails = prefs.PaymentDetails;
                await prefsRepo.UpdateAsync(existing.Id.ToString(), existing);
            }
            else
            {
                prefs.UserId = userId;
                await prefsRepo.AddAsync(prefs);
            }
            return Ok();
        }

        [HttpGet("payment-info")]
        public async Task<IActionResult> GetPaymentInfo([FromQuery] Guid userId, [FromServices] IMongoRepository<UserPreferences> prefsRepo)
        {
            var prefs = (await prefsRepo.FindAsync(p => p.UserId == userId)).FirstOrDefault();
            if (prefs == null) return NotFound();
            return Ok(new { prefs.PreferredPaymentMethod, prefs.PaymentDetails });
        }
    }
} 