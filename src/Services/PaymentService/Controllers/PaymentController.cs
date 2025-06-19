using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Transport.Shared.Entities.MongoDB;
using Transport.Shared.Enums;

namespace PaymentService.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentController : ControllerBase
{
    private readonly IMongoCollection<PaymentLog> _paymentLogs;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IMongoDatabase database, ILogger<PaymentController> logger)
    {
        _paymentLogs = database.GetCollection<PaymentLog>("paymentLogs");
        _logger = logger;
    }

    [HttpPost("process")]
    public async Task<IActionResult> ProcessPayment([FromBody] PaymentProcessDto paymentDto)
    {
        try
        {
            // In a real implementation, you would integrate with a payment gateway here
            // For now, we'll simulate a successful payment

            var paymentLog = new PaymentLog
            {
                UserId = Guid.Parse(paymentDto.UserId),
                TripId = !string.IsNullOrEmpty(paymentDto.TripId) ? Guid.Parse(paymentDto.TripId) : null,
                Amount = paymentDto.Amount,
                Status = "Completed",
                Type = paymentDto.PaymentType,
                CreatedAt = DateTime.UtcNow
            };

            await _paymentLogs.InsertOneAsync(paymentLog);

            _logger.LogInformation($"Payment processed for user {paymentDto.UserId} for {paymentDto.Amount:C}");

            return Ok(new 
            { 
                Message = "Payment processed successfully", 
                PaymentId = paymentLog.Id.ToString(),
                Status = "Completed"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("reverse")]
    public async Task<IActionResult> ReversePayment([FromBody] PaymentReversalDto reversalDto)
    {
        try
        {
            var payment = await _paymentLogs.Find(p => p.Id == ObjectId.Parse(reversalDto.PaymentId)).FirstOrDefaultAsync();
            if (payment == null)
            {
                return NotFound("Payment not found");
            }

            if (payment.Status != "Completed")
            {
                return BadRequest("Payment cannot be reversed - not in completed status");
            }

            var update = Builders<PaymentLog>.Update
                .Set(p => p.Status, "Reversed")
                .Set(p => p.Type, "Reversal");

            var result = await _paymentLogs.UpdateOneAsync(p => p.Id == ObjectId.Parse(reversalDto.PaymentId), update);
            if (result.MatchedCount == 0)
            {
                return NotFound("Payment not found");
            }

            _logger.LogInformation($"Payment reversed: {reversalDto.PaymentId} - Reason: {reversalDto.Reason}");

            return Ok(new 
            { 
                Message = "Payment reversed successfully", 
                PaymentId = reversalDto.PaymentId,
                Status = "Reversed"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reversing payment");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserPayments(string userId, [FromQuery] string? status = null)
    {
        try
        {
            var filter = Builders<PaymentLog>.Filter.Eq(p => p.UserId, Guid.Parse(userId));
            if (!string.IsNullOrEmpty(status))
            {
                filter &= Builders<PaymentLog>.Filter.Eq(p => p.Status, status);
            }

            var payments = await _paymentLogs.Find(filter)
                .SortByDescending(p => p.CreatedAt)
                .ToListAsync();

            return Ok(payments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payments");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("payment/{paymentId}")]
    public async Task<IActionResult> GetPaymentById(string paymentId)
    {
        try
        {
            var payment = await _paymentLogs.Find(p => p.Id == ObjectId.Parse(paymentId)).FirstOrDefaultAsync();
            if (payment == null)
            {
                return NotFound("Payment not found");
            }

            return Ok(payment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("analytics/user/{userId}")]
    public async Task<IActionResult> GetUserPaymentAnalytics(string userId)
    {
        try
        {
            var payments = await _paymentLogs.Find(p => p.UserId == Guid.Parse(userId)).ToListAsync();
            
            var analytics = new
            {
                TotalPayments = payments.Count,
                TotalAmount = payments.Where(p => p.Status == "Completed").Sum(p => p.Amount),
                CompletedPayments = payments.Count(p => p.Status == "Completed"),
                ReversedPayments = payments.Count(p => p.Status == "Reversed"),
                PendingPayments = payments.Count(p => p.Status == "Pending"),
                LastPaymentDate = payments.OrderByDescending(p => p.CreatedAt).FirstOrDefault()?.CreatedAt
            };

            return Ok(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment analytics");
            return StatusCode(500, "Internal server error");
        }
    }
}

// DTOs for Payment Service
public class PaymentProcessDto
{
    public string UserId { get; set; } = string.Empty;
    public string? TripId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentType { get; set; } = string.Empty;
}

public class PaymentReversalDto
{
    public string PaymentId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
} 