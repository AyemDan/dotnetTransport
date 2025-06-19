using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Transport.Domain.Entities.MongoDB;

public class Subscription
{
    [BsonId]
    public ObjectId Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RouteId { get; set; }
    public string Period { get; set; } // e.g., "Monthly", "Weekly"
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } // e.g., Active, Cancelled, Expired
    public decimal Amount { get; set; }
    public string PaymentStatus { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 