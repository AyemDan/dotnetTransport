using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Transport.Application.Services;
using Transport.Domain.Entities.MongoDB;
using Transport.API.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Transport.API.Controllers
{
    [ApiController]
    [Route("api/subscription")]
    [Authorize(Roles = "OrganizationAdmin")]
    public class SubscriptionController : ControllerBase
    {
        private readonly SubscriptionService _subscriptionService;
        public SubscriptionController(SubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSubscriptions()
        {
            var subs = await _subscriptionService.GetAllSubscriptionsAsync();
            return Ok(subs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubscriptionById(string id)
        {
            var sub = await _subscriptionService.GetSubscriptionByIdAsync(id);
            if (sub == null) return NotFound();
            return Ok(sub);
        }

        [HttpPost]
        public async Task<IActionResult> AddSubscription([FromBody] SubscriptionDto dto)
        {
            var sub = new Subscription
            {
                UserId = dto.UserId,
                RouteId = dto.RouteId,
                Period = dto.Period,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                Amount = dto.Amount,
                PaymentStatus = dto.PaymentStatus,
                CreatedAt = System.DateTime.UtcNow
            };
            await _subscriptionService.AddSubscriptionAsync(sub);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubscription(string id, [FromBody] SubscriptionDto dto)
        {
            var sub = new Subscription
            {
                UserId = dto.UserId,
                RouteId = dto.RouteId,
                Period = dto.Period,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                Amount = dto.Amount,
                PaymentStatus = dto.PaymentStatus,
                CreatedAt = System.DateTime.UtcNow
            };
            await _subscriptionService.UpdateSubscriptionAsync(id, sub);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubscription(string id)
        {
            await _subscriptionService.DeleteSubscriptionAsync(id);
            return Ok();
        }
    }
} 