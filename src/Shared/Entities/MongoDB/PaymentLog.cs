using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Transport.Shared.Entities.MongoDB;

public class PaymentLog
{
    [BsonId]
    public ObjectId Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? TripId { get; set; }
    public string OrganizationId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty; // e.g., Completed, Refunded, Reversed, Pending
    public string Type { get; set; } = string.Empty; // e.g., TapIn, TapOutRefund
    public string PaymentMethod { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    public DateTime? ReversedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? ReversalReason { get; set; }
} 