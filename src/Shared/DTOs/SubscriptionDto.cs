using System;

namespace Transport.Shared.DTOs;

public class SubscriptionDto
{
    public Guid UserId { get; set; }
    public Guid RouteId { get; set; }
    public string Period { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
} 