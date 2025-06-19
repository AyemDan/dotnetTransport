using System;

namespace Transport.Shared.DTOs;

public class PaymentInfoDto
{
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
} 