using System;

namespace Transport.Shared.DTOs;

public class RFIDCardDto
{
    public string CardNumber { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string CardType { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
    public DateTime LastUsed { get; set; }
    public string? LastLocation { get; set; }
    public int UsageCount { get; set; }
    public string? AssignedVehicle { get; set; }
    public string? AssignedRoute { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
} 