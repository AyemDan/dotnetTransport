using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Transport.Shared.Entities.MongoDB;
using Transport.Shared.Enums;
using Transport.Shared.Services;

namespace PaymentService.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
    {
        _paymentService = paymentService;
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
                TripId = !string.IsNullOrEmpty(paymentDto.TripId)
                    ? Guid.Parse(paymentDto.TripId)
                    : null,
                Amount = paymentDto.Amount,
                Type = paymentDto.PaymentType,
            };

            var processedPayment = await _paymentService.ProcessPaymentAsync(paymentLog);

            _logger.LogInformation(
                $"Payment processed for user {paymentDto.UserId} for {paymentDto.Amount:C}"
            );

            return Ok(
                new
                {
                    Message = "Payment processed successfully",
                    PaymentId = processedPayment.Id.ToString(),
                    Status = "Completed",
                }
            );
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
            var success = await _paymentService.ReversePaymentAsync(reversalDto.PaymentId, reversalDto.Reason);
            if (!success)
            {
                return BadRequest("Payment cannot be reversed - not in completed status or not found");
            }

            _logger.LogInformation(
                $"Payment reversed: {reversalDto.PaymentId} - Reason: {reversalDto.Reason}"
            );

            return Ok(
                new
                {
                    Message = "Payment reversed successfully",
                    PaymentId = reversalDto.PaymentId,
                    Status = "Reversed",
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reversing payment");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserPayments(
        string userId,
        [FromQuery] string? status = null
    )
    {
        try
        {
            var payments = await _paymentService.GetUserPaymentsAsync(Guid.Parse(userId), status);
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
            var payment = await _paymentService.GetPaymentByIdAsync(paymentId);
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
            var analytics = await _paymentService.GetUserPaymentAnalyticsAsync(Guid.Parse(userId));
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
