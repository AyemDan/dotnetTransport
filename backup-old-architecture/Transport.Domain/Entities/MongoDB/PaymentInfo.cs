using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Transport.Domain.Entities.MongoDB;

public class PaymentLog
{
    [BsonId]
    public ObjectId Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? TripId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } // e.g., Completed, Refunded
    public string Type { get; set; } // e.g., TapIn, TapOutRefund
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
